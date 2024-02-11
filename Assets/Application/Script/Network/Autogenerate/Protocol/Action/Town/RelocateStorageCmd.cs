using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class RelocateStorageCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "RelocateStorage.do",
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
		public async static Task<Command.Result> CreateTask(RelocateStorageRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(RelocateStorageRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<RelocateStorageResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class RelocateStorageRequest : Request
	{
		public RelocateStorageRequest(StorageData storageData, List<ChainData> chains){
			this.storageData = storageData;
			this.chains = chains;

		}
		//---- リクエスト変数定義 ----
		public StorageData storageData; //倉庫
		public List<ChainData> chains; //再配置後のChain

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class RelocateStorageResponse : Response
	{
		//---- レスポンス変数定義 ----

	}

}
