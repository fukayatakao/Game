using Project.Mst;
using Project.Network;
using System.Collections.Generic;
using Project.Http.Mst;


#if USE_OFFLINE

namespace Project.Server {
	public static class StrategyMapTool {
		public static UserStrategyMapTable strategyMap = new UserStrategyMapTable();
		public static Dictionary<int, PlatoonData> invader = new Dictionary<int, PlatoonData>();
		public static Dictionary<int, PlatoonData> defender = new Dictionary<int, PlatoonData>();

		/// <summary>
		/// セーブする
		/// </summary>
		public static void Save() {
			UserDB.Instance.strategyMapTable.Clear();
			UserDB.Instance.strategyMapTable.Add(strategyMap);

			UserDB.Instance.strategyLocationTable.Clear();
			foreach(var pair in invader) {
				UserStrategyLocationTable location = new UserStrategyLocationTable();
				location.userId = 0;
				location.mapId = strategyMap.mapId;
				location.nodeId = pair.Key;
				location.platoonId = pair.Value.id;
				location.attitude = (int)STRATEGY_ATTITUDE.INVADER;
				UserDB.Instance.strategyLocationTable.Add(location);
			}

			foreach (var pair in defender) {
				UserStrategyLocationTable location = new UserStrategyLocationTable();
				location.userId = 0;
				location.mapId = strategyMap.mapId;
				location.nodeId = pair.Key;
				location.platoonId = pair.Value.id;
				location.attitude = (int)STRATEGY_ATTITUDE.DEFENDER;
				UserDB.Instance.strategyLocationTable.Add(location);
			}
		}

		/// <summary>
		/// 軍団の編成状態を作る
		/// </summary>
		public static void LoadNewMap(int userId, int mapId) {
			strategyMap = UserStrategyMapTableDAO.SelectByUserId(userId);

			invader.Clear();
			defender.Clear();
			//指定マップの記録がある
			if (strategyMap != null && strategyMap.mapId == mapId) {
				var list = UserStrategyLocationTableDAO.SelectByUserIdMapId(userId, mapId);
				var invaderLocation = list.FindAll(x => x.attitude == (int)STRATEGY_ATTITUDE.INVADER);
				for(int i = 0, max = invaderLocation.Count; i < max; i++) {
					invader[invaderLocation[i].nodeId] = PlatoonTool.platoonDict[invaderLocation[i].platoonId];
				}

				var defenderLocation = list.FindAll(x => x.attitude == (int)STRATEGY_ATTITUDE.DEFENDER);
				for (int i = 0, max = defenderLocation.Count; i < max; i++) {
					defender[defenderLocation[i].nodeId] = PlatoonTool.LoadEnemyPlatoon(defenderLocation[i].platoonId);
				}
				return;
			}
			//新規でデータ作成
			if (strategyMap == null) {
				UserStrategyMapTable table = new UserStrategyMapTable();
				UserStrategyMapTableDAO.Insert(table);
				strategyMap = table;
			}

			strategyMap.userId = userId;            //1ユーザーに戦略マップの状態を保持出来るのは1ユーザーにつき１つ
			strategyMap.mapId = mapId;              //
			strategyMap.json = null;                //pvpなどでユーザーが経路マップを作ったときはここに保存する
			strategyMap.turn = 0;

			//攻撃側の部隊と配置状況
			PlatoonTool.CreateCorps();

			//マップIDが一致する配置データを抜き出す
			List<MstStrategyLocationData> masterlocation = BaseDataManager.GetList<MstStrategyLocationData>().FindAll(m => m.Map == mapId);
			for (int i = 0, max = masterlocation.Count; i < max; i++) {
				defender[masterlocation[i].Node] = PlatoonTool.LoadEnemyPlatoon(masterlocation[i].Platoon);
			}


			Save();
		}

		/// <summary>
		/// クライアントに送る用の攻撃側部隊配置情報を作成
		/// </summary>
		public static List<StrategyLocationData> CreateInvaderLocation() {
			var list = new List<StrategyLocationData>();
			foreach (var inv in invader) {
				StrategyLocationData data = new StrategyLocationData();

				data.platoon = inv.Value;
				data.nodeId = inv.Key;
				list.Add(data);
			}
			return list;
		}

		/// <summary>
		/// クライアントに送る用の防衛側部隊配置情報を作成
		/// </summary>
		public static List<StrategyLocationData> CreateDefenderLocation() {
			var list = new List<StrategyLocationData>();
			foreach (var def in defender) {
				StrategyLocationData data = new StrategyLocationData();

				data.platoon = def.Value;
				data.nodeId = def.Key;
				list.Add(data);
			}
			return list;
		}


	}


}
#endif
