using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class LoginCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "Login.do",
				Server = Command.Config.ServerRoot[ "sim" ],
				UserAgent = Command.Config.UserAgent[ "sim" ],
				EncryptKey = Command.Config.DefaultEncryptKey,
				TimeoutTime = 10f,
				RetryCount = CommandSetting.DEFAULT_RETRY_COUNT,
				IsPostMethod = false,
				IsOffline = true,

#if USE_OFFLINE
				OfflineFunc = OfflineFunc,
#endif
				Parser = Parse,
			};
			return setting;
		}
		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static Task<Command.Result> CreateTask(LoginRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(LoginRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<LoginResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class LoginRequest : Request
	{
		public LoginRequest(string account, string secure_id, string check_code, string uuid, int platform, int language, long time_offset){
			this.account = account;
			this.secure_id = secure_id;
			this.check_code = check_code;
			this.uuid = uuid;
			this.platform = platform;
			this.language = language;
			this.time_offset = time_offset;

		}
		//---- リクエスト変数定義 ----
		public string account; //ログインID
		public string secure_id; //セキュアID
		public string check_code; //クライアントコード
		public string uuid; //UUID
		public int platform; //プラットフォーム PlatformDef
		public int language; //言語 LanguageDef
		public long time_offset; //端末のローカル時と標準時の差(ms)

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class LoginResponse : Response
	{
		//---- レスポンス変数定義 ----
		public string sid; //
		public int user_id; //ユーザーID
		public List<Project.Mst.MstVersion> basedata_version; //ベースデータバージョンリスト
		public int safety_net_interval; //セフティネットフラグ
		public List<int> tutorial; //残りチュートリアル

	}

}
