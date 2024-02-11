using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class BattleMainCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "BattleMain.do",
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
		public async static Task<Command.Result> CreateTask(BattleMainRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(BattleMainRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<BattleMainResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class BattleMainRequest : Request
	{
		public BattleMainRequest(int invaderId, int defenderId, int nodeId, int stageId){
			this.invaderId = invaderId;
			this.defenderId = defenderId;
			this.nodeId = nodeId;
			this.stageId = stageId;

		}
		//---- リクエスト変数定義 ----
		public int invaderId; //攻撃側部隊ID
		public int defenderId; //防衛側部隊ID
		public int nodeId; //ノードID
		public int stageId; //ステージID

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class BattleMainResponse : Response
	{
		//---- レスポンス変数定義 ----
		public BattleSituationData situation; //バトル情報

	}

}
