using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class StrategyMainCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "StrategyMain.do",
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
		public async static Task<Command.Result> CreateTask(StrategyMainRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(StrategyMainRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<StrategyMainResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class StrategyMainRequest : Request
	{
		public StrategyMainRequest(){

		}
		//---- リクエスト変数定義 ----

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class StrategyMainResponse : Response
	{
		//---- レスポンス変数定義 ----
		public StrategySituationData situation; //情報

	}

}
