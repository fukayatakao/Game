using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class QuestBattleCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "QuestBattle.do",
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
		public async static Task<Command.Result> CreateTask(QuestBattleRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(QuestBattleRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<QuestBattleResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class QuestBattleRequest : Request
	{
		public QuestBattleRequest(int invaderId, int questId){
			this.invaderId = invaderId;
			this.questId = questId;

		}
		//---- リクエスト変数定義 ----
		public int invaderId; //攻撃側部隊ID
		public int questId; //クエストID

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class QuestBattleResponse : Response
	{
		//---- レスポンス変数定義 ----
		public BattleSituationData situation; //バトル情報

	}

}