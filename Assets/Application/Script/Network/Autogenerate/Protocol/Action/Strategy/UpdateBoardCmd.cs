using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class UpdateBoardCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "UpdateBoard.do",
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
		public async static Task<Command.Result> CreateTask(UpdateBoardRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(UpdateBoardRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<UpdateBoardResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class UpdateBoardRequest : Request
	{
		public UpdateBoardRequest(List<StrategyLocationData> invaderLocation, List<StrategyLocationData> defenderLocation, int turn, List<int> available){
			this.invaderLocation = invaderLocation;
			this.defenderLocation = defenderLocation;
			this.turn = turn;
			this.available = available;

		}
		//---- リクエスト変数定義 ----
		public List<StrategyLocationData> invaderLocation; //攻撃側配置
		public List<StrategyLocationData> defenderLocation; //守備配置
		public int turn; //戦略マップのターン
		public List<int> available; //現ターンの残り行動可能部隊

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class UpdateBoardResponse : Response
	{
		//---- レスポンス変数定義 ----

	}

}
