using Project.Network;
using System.Collections.Generic;

#if USE_OFFLINE

namespace Project.Server {
	public partial class DummyServer {
		/// <summary>
		/// 戦域シーン開始通信
		/// </summary>
		/// <remarks>
		/// pveで攻撃側がプレイヤーを想定して仮実装
		/// </remarks>
		public static Response StrategyMainOffline(Request req) {
			StrategyMainRequest request = req as StrategyMainRequest;


			StrategyMainResponse response = new StrategyMainResponse();
			int userId = 0;
			int mapId = 1;

			//所持中の部隊情報を送る
			PlatoonTool.CreateCorps();
			response.situation = new StrategySituationData();
			response.situation.invader = new List<PlatoonData>(PlatoonTool.platoonDict.Values);

			//記録されているマップのロード。なければ作成
			StrategyMapTool.LoadNewMap(userId, mapId);

			//セーブされている攻撃側配置状況を送る
			response.situation.invaderLocation = StrategyMapTool.CreateInvaderLocation();
			response.situation.defenderLocation = StrategyMapTool.CreateDefenderLocation();


			response.situation.mapId = mapId;
			response.situation.turn = StrategyMapTool.strategyMap.turn;
			response.situation.available = StrategyMapTool.strategyMap.available != null ? StrategyMapTool.strategyMap.available : new List<int>();

			UserDB.Save();

			return response;
		}
		/// <summary>
		/// 盤面配置更新
		/// </summary>
		public static Response UpdateBoard(Request req) {
			UpdateBoardRequest request = (UpdateBoardRequest)req;
			StrategyMapTool.invader.Clear();
			for (int i = 0, max = request.invaderLocation.Count; i < max; i++) {
				StrategyLocationData data = request.invaderLocation[i];
				StrategyMapTool.invader[data.nodeId] = data.platoon;
			}
			StrategyMapTool.defender.Clear();
			for (int i = 0, max = request.defenderLocation.Count; i < max; i++) {
				StrategyLocationData data = request.defenderLocation[i];
				StrategyMapTool.defender[data.nodeId] = data.platoon;
			}

			StrategyMapTool.strategyMap.turn = request.turn;
			StrategyMapTool.strategyMap.available = request.available;

			StrategyMapTool.Save();

			UserDB.Save();

			UpdateBoardResponse response = new UpdateBoardResponse();
			return response;
		}

	}
}
#endif
