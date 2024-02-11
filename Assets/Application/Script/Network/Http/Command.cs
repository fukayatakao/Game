using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Network {
	public class CommandSetting {
		public const float DEFAULT_TIMEOUT_TIME = 15f;
		public const int DEFAULT_RETRY_COUNT = 0;

		public string Api;
		public string Server;
		public string EncryptKey;
		public string UserAgent;
		public float TimeoutTime;
		public bool IsPostMethod;
		public uint RetryCount;
		public bool IsParallel;
		/// <summary>
		/// JsonからレスポンスにParse
		/// </summary>
		public System.Func<string, Response> Parser;
#if USE_OFFLINE
		//オフラインフラグ
		public bool IsOffline = true;
		//オフラインhook関数
		public System.Func<Request, Response> OfflineFunc;
#endif
	}


	public class Command {
		/// <summary>
		/// 通信接続設定
		/// </summary>
		public class Config {
			public static Dictionary<string, string> ServerRoot = new Dictionary<string, string>();
			public static Dictionary<string, string> UserAgent = new Dictionary<string, string>();
			public static string DefaultEncryptKey;
			public static string AccountEncryptKey;

			public static string Proxy = "localhost:8080";
		}

		/// <summary>
		/// 通信結果
		/// </summary>
		public class Result {
			//レスポンスデータ
			public Response Response;
			//通信自体の結果
			public System.Net.WebExceptionStatus ExceptionStatus;

			/// <summary>
			/// 通信が正常に終了したか
			/// </summary>
			public bool IsSuccess() {
				//通信エラーの場合
				if (ExceptionStatus != System.Net.WebExceptionStatus.Success)
					return false;
				//サーバエラーの場合
				if (Response.res_code != Response.RES_CODE_SUCCESS)
					return false;

				//エラーがないので正常終了と判断
				return true;
			}
			/// <summary>
			/// コールバック呼び出し
			/// </summary>
			public void ExecCallback(System.Action<Response> success, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
				if (IsSuccess()) {
					if (success != null) { success(Response); } else { Debug.Log("success"); }
				} else {
					if (failure != null) { failure(Response, ExceptionStatus); } else { Debug.Log("failuer"); }
				}
			}

		}
		//最大並列実行数
		private const int MAX_PARALLEL_COUNT = 3;

		//@note サーバ側でリクエスト順を知るのに使うカウンタ。アプリ再起動したときにリクエスト順が前後しないように時間を初期値にしてカウントアップさせる。
		//　　　（固定値を使うと再起動でリセットかかると再起動前後でリクエスト順が正しくなくなる
		//最初に時間を入れて以降は1ずつカウントアップ
		private static long sequenceCounter_ = System.DateTime.Now.Ticks;
		//サーバ側の負荷が上がったときなど待機時間を増やしてリクエスト頻度を減らすための遅延時間
		private static int delayTime_;
		//直列実行用のカレントTask
		private static Task currentCmd = Task.CompletedTask;
		//同時最大数制御用
		private static SemaphoreSlim semaphore_ = new SemaphoreSlim(MAX_PARALLEL_COUNT);
		//Taskキャンセル用のトークン
		private static CancellationTokenSource cancelTokenSource_ = new CancellationTokenSource();
		//サーバ側に送るユニークカウンタ
		private long unique_;
		//CommandSetting Setting;


		private readonly object sequenceCounterLock_ = new object();
		private readonly object delayTimeLock_ = new object();

		/// <summary>
		/// リクエストに対してユニークな順値を割り振る
		/// </summary>
		private void RequestSigned(Request request) {
			//新規通信
			if (request._tm_ == 0) {
				lock (sequenceCounterLock_) {
					++sequenceCounter_;
					this.unique_ = sequenceCounter_;
				}
				request._tm_ = this.unique_;
				//リトライ
			} else {
			}
		}

		/// <summary>
		/// 非同期通信処理
		/// </summary>
		public async Task<Result> CorrespondAsync(Request request, CommandSetting setting) {
			RequestSigned(request);
			//全体で共有のトークンを使う。個別にキャンセル制御したい場合は別途相談
			CancellationToken token = cancelTokenSource_.Token;
			try {
				Task<Result> correspond;
				//並列実行
				if (setting.IsParallel) {
					//セマフォロックを取得できるまで待つ
					await semaphore_.WaitAsync();
					try {
						correspond = CorrespondAsyncImpl(request, setting, token);
						await correspond;
					} finally {
						//セマフォロック解放
						semaphore_.Release();
					}
					//直列実行
				} else {
					correspond = currentCmd.ContinueWith(a => CorrespondAsyncImpl(request, setting, token), token).Unwrap();
					currentCmd = correspond;
					await correspond;
				}
				return correspond.Result;
				//外からキャンセルされた場合
			} catch (System.OperationCanceledException e) {
				Debug.LogWarning($"{nameof(System.OperationCanceledException)} thrown with message: {e.Message}");
				//通信キャンセルのステータスで返す
				Result result = new Result();
				result.ExceptionStatus = System.Net.WebExceptionStatus.RequestCanceled;
				return result;
			}
		}


		/// <summary>
		/// 非同期通信処理
		/// </summary>
		private async Task<Result> CorrespondAsyncImpl(Request request, CommandSetting setting, CancellationToken token) {
			Result result = null;
			for (int i = 0; i <= setting.RetryCount; i++) {
				//通信開始
				DebugLog("<color=#70cc70>[CMD] wait:" + this.GetType().ToString() + "</color>");
				var ret = await Web.HttpCommand(setting.Server + setting.Api, request, setting.Parser, setting.TimeoutTime, delayTime_);

				result = new Result() { Response = ret.Item1, ExceptionStatus = ret.Item2 };
				//外からキャンセルされたらここで例外投げて終了
				//通信自体は成否出るまで行って各種コールバックは実行しないで抜ける。ここなら明示的に破棄しなければいけないインスタンスもなし。
				token.ThrowIfCancellationRequested();

				//通信に成功したらコールバック呼んで終了
				if (result.ExceptionStatus == System.Net.WebExceptionStatus.Success) {
					Response respons = result.Response;

					//通信は正常に行われたがサーバ側でエラーとされた場合（送ったパラメータが異常、サーバ側の処理エラーなど
					if (respons.res_code != Response.RES_CODE_SUCCESS) {
						DebugLog(string.Format("<color=#c07070>[HTTP] {0} - Request Error. res={1} </color>", setting.Api, respons.res_code.ToString()));
					} else {
						//@note 以降のリクエストは遅延時間分待機してから実行される->リクエスト頻度が減るのでサーバ側の負荷が軽減させる仕組み。
						//サーバ側の状態に応じて遅延時間(ミリ秒)をセットする。
						lock (delayTimeLock_) {
							Command.delayTime_ = (int)(respons.client_wait * 1000);
						}
					}
					return result;
				}
			}
			// エラー通知表示
			Debug.LogError(result.ExceptionStatus);

			return result;
		}


		/// <summary>
		/// 通信の強制中断
		/// </summary>
		public static void Abort() {
			//トークンに対してキャンセルを通知後に解放
			cancelTokenSource_.Cancel();
			cancelTokenSource_.Dispose();       //明示的に呼ぶ必要あり

			//新しいトークンを発行
			cancelTokenSource_ = new CancellationTokenSource();
		}

		/// <summary>
		/// 通信中か
		/// </summary>
		public static bool IsAccessing() {
			//直列処理が終わっていないか並列処理のスレッド残り数が最大数でない場合はタスクが残っている=通信中とみなす
			return !currentCmd.IsCompleted || semaphore_.CurrentCount != MAX_PARALLEL_COUNT;
		}

		/// <summary>
		/// オフライン処理
		/// </summary>
		public static Response Offline(Request req) {
			Debug.LogError("not found offline function:" + req.GetType());
			return new Response();
		}


		/// <summary>
		/// 通信ログを出力
		/// </summary>
		/// <remarks>
		/// DEVELOP_BUILDの定義があるときのみ出力する
		/// </remarks>
		[System.Diagnostics.Conditional("DEVELOP_BUILD")]
		private static void DebugLog(string text) {
#if !DISABLE_HTTP_LOG
			Debug.Log(text);
#endif
		}

	}
}
