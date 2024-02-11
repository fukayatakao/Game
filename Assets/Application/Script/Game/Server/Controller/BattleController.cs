using Project.Game;
using Project.Mst;
using Project.Network;
using System.Collections.Generic;
using System.Linq;
#if USE_OFFLINE

namespace Project.Server {
	public partial class DummyServer {
		/// <summary>
		/// バトル開始通信
		/// </summary>
		public static Response BattleMainOffline(Request req) {
			var request = req as BattleMainRequest;

			BattleMainResponse response = new BattleMainResponse();
			PlatoonTool.CreateCorps();
			response.situation = new BattleSituationData();
			response.situation.invader = PlatoonTool.platoonDict[request.invaderId];
			MstEnemyCorps mst = MstEnemyCorps.GetData(request.defenderId);
			response.situation.defender = PlatoonTool.LoadEnemyPlatoon(mst);
			response.situation.enemyAI = mst.Ai;
			response.situation.stageId = 0;//@todo nodeIdからstageのidを探す
			response.situation.nodeId = request.nodeId;
			return response;
		}

		/// <summary>
		/// 編集開始通信
		/// </summary>
		public static Response QuestBattleOffline(Request req) {
			var request = req as QuestBattleRequest;
			MstQuestData quest = MstQuestData.GetData(request.questId);
			QuestBattleResponse response = new QuestBattleResponse();
			PlatoonTool.CreateCorps();
			response.situation = new BattleSituationData();
			response.situation.invader = PlatoonTool.platoonDict[request.invaderId];

			MstEnemyCorps mst = MstEnemyCorps.GetData(quest.Platoon);

			response.situation.defender = PlatoonTool.LoadEnemyPlatoon(mst);
			response.situation.enemyAI = mst.Ai;
			response.situation.stageId = quest.Map;

			return response;
		}

#if DEVELOP_BUILD
		/// <summary>
		/// シーン開始通信
		/// </summary>
		public static Response DebugBattleMainOffline(Request req) {
			var request = req as DebugBattleMainRequest;

			DebugBattleMainResponse response = new DebugBattleMainResponse();
			PlatoonTool.CreateCorps();
			//ユーザーデータの先頭の部隊を適当に返す
			response.situation = new BattleSituationData();
			response.situation.invader = PlatoonTool.platoonDict.Values.First();


			if (Game.EditPlatoon.IsExist()) {
				var mst = EditPlatoon.Load().CreateCorps();
				response.situation.defender = PlatoonTool.LoadEnemyPlatoon(mst);
			} else {
				response.situation.defender = PlatoonTool.LoadEnemyPlatoon(1);
			}
			response.situation.enemyAI = "Platoon/AI/basic";
			response.situation.stageId = 0;
			response.situation.nodeId = 0;
			return response;
		}
#endif
		/// <summary>
		/// 編集開始通信
		/// </summary>
		public static Response OrganizeMainOffline(Request req) {
			var request = req as OrganizeMainRequest;

			OrganizeMainResponse response = new OrganizeMainResponse();

			PlatoonTool.CreateCorps();

			response.platoons = new List<PlatoonData>(PlatoonTool.platoonDict.Values);;
			response.leaders = PlatoonTool.reserveLeaders;
			response.reserves = PlatoonTool.reserveCharas;
			response.stageId = 99;
			return response;
		}

		/// <summary>
		/// バトル勝利通信
		/// </summary>
		public static Response BattleResultWinOffline(Request req) {
			var request = req as BattleResultWinRequest;
			int mapId = 1;


			StrategyMapTool.defender.Remove(request.nodeId);
			foreach(var pair in StrategyMapTool.invader) {
				if(pair.Value.id == request.invaderId) {
					StrategyMapTool.invader[request.nodeId] = pair.Value;
					StrategyMapTool.invader.Remove(pair.Key);
					break;
				}
			}

			BattleResultWinResponse response = new BattleResultWinResponse();
			//所持中の部隊情報を送る
			response.situation = new StrategySituationData();
			response.situation.invader = new List<PlatoonData>(PlatoonTool.platoonDict.Values);

			//セーブされている攻撃側配置状況を送る
			response.situation.invaderLocation = StrategyMapTool.CreateInvaderLocation();
			response.situation.defenderLocation = StrategyMapTool.CreateDefenderLocation();


			response.situation.mapId = mapId;
			response.situation.turn = StrategyMapTool.strategyMap.turn;
			response.situation.available = StrategyMapTool.strategyMap.available;

			UserDB.Save();


			return response;
		}
		/// <summary>
		/// バトル敗北通信
		/// </summary>
		public static Response BattleResultLoseOffline(Request req) {
			var request = req as BattleMainRequest;

			BattleMainResponse response = new BattleMainResponse();
			return response;
		}
		/// <summary>
		/// 部隊編成更新通信
		/// </summary>
		public static Response UpdateCorpsOffline(Request req) {
			var request = req as UpdateCorpsRequest;
			Dictionary<int, UserPlatoonTable> platoonTableDict = UserPlatoonTableDAO.SelectByIds(new List<int>(request.platoons.Select(x => x.id))).ToDictionary(x => x.id);

			foreach (PlatoonData platoon in request.platoons) {
				//小隊情報の更新
				UserPlatoonTable platoonTable = platoonTableDict[platoon.id];
				platoonTable.name = platoon.name;
				platoonTable.experience = platoon.experience;
				platoonTable.unitId1 = platoon.squads.Count > 0 ? platoon.squads[0].unitId : 0;
				platoonTable.unitId2 = platoon.squads.Count > 1 ? platoon.squads[1].unitId : 0;
				platoonTable.unitId3 = platoon.squads.Count > 2 ? platoon.squads[2].unitId : 0;
				//分隊ごとのキャラクター情報更新
				List<UserPlatoonMemberTable> units = new List<UserPlatoonMemberTable>();
				for (int i = 0, max = platoon.squads.Count; i < max; i++) {
					for (int j = 0, max2 = platoon.squads[i].members.Count; j < max2; j++) {
						//無名キャラの場合は登録不要
						if (platoon.squads[i].members[j] == null)
							continue;
						UserPlatoonMemberTable platoonCharacter = new UserPlatoonMemberTable();
						platoonCharacter.platoonId = platoon.id;
						platoonCharacter.abreast = i;
						platoonCharacter.number = j;
						platoonCharacter.charaId = platoon.squads[i].members[j].id;

						units.Add(platoonCharacter);
					}
				}
				UserPlatoonUnitTableDAO.UpdateCharacters(platoon.id, units);

				List<UserPlatoonLeaderTable> leaders = new List<UserPlatoonLeaderTable>();
				for (int i = 0, max = platoon.squads.Count; i < max; i++) {
					if (platoon.squads[i].leader == null)
						continue;

					UserPlatoonLeaderTable platoonLeader = new UserPlatoonLeaderTable();
					platoonLeader.platoonId = platoon.id;
					platoonLeader.abreast = i;
					platoonLeader.leaderId = platoon.squads[i].leader.id;
					leaders.Add(platoonLeader);
				}
				UserPlatoonLeaderTableDAO.UpdateLeaders(platoon.id, leaders);
			}

			UserDB.Save();
			UpdateCorpsResponse response = new UpdateCorpsResponse();
			return response;
		}
	}
}
#endif
