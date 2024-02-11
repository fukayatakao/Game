using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class UpdateServiceCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "UpdateService.do",
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
		public async static Task<Command.Result> CreateTask(UpdateServiceRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(UpdateServiceRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<UpdateServiceResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class UpdateServiceRequest : Request
	{
		public UpdateServiceRequest(ServiceData serviceData){
			this.serviceData = serviceData;

		}
		//---- リクエスト変数定義 ----
		public ServiceData serviceData; //更新された倉庫

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class UpdateServiceResponse : Response
	{
		//---- レスポンス変数定義 ----

	}

}
