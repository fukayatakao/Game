using Project.Http.Mst;
using Project.Mst;
using Project.Network;
using System.Collections.Generic;
using System.Linq;
#if USE_OFFLINE

namespace Project.Server {
	public static class PlatoonTool {
		public static Dictionary<int, PlatoonData> platoonDict = new Dictionary<int, PlatoonData>();
		public static List<CharacterData> reserveCharas;
		public static List<LeaderData> reserveLeaders;

		/// <summary>
		/// 軍団の編成状態を作る
		/// </summary>
		public static void CreateCorps() {
			//ユーザーが保持している全小隊を取得
			List<UserPlatoonTable> userPlatoons = UserPlatoonTableDAO.SelectAll();
			//全キャラを取得してキャラIDのDictionaryを作る
			Dictionary<int, UserMemberTable> allCharacter = UserCharacterTableDAO.SelectAll().ToDictionary(mst => mst.id);
			Dictionary<int, UserLeaderTable> allLeaders = UserLeaderTableDAO.SelectAll().ToDictionary(mst => mst.id);
			List<int> assignedCharacter = new List<int>();
			List<int> assignedLeader = new List<int>();


			List<PlatoonData> platoons = new List<PlatoonData>();
			for (int i = 0, max = userPlatoons.Count; i < max; i++) {
				int platoonId = userPlatoons[i].id;
				//何度もSelectは本当のDBでは重いので変える必要がある
				List<UserPlatoonMemberTable> units = UserPlatoonUnitTableDAO.SelectByPlatoonId(platoonId);
				List<UserPlatoonLeaderTable> leader = UserPlatoonLeaderTableDAO.SelectByPlatoonId(platoonId);


				PlatoonData platoon = new PlatoonData();
				platoon.id = platoonId;
				platoon.name = userPlatoons[i].name;
				platoon.experience = userPlatoons[i].experience;

				platoon.squads = new List<SquadData>() { new SquadData(), new SquadData(), new SquadData() };
				platoon.squads[(int)Abreast.First].unitId = userPlatoons[i].unitId1;
				platoon.squads[(int)Abreast.Second].unitId = userPlatoons[i].unitId2;
				platoon.squads[(int)Abreast.Third].unitId = userPlatoons[i].unitId3;

				//リーダー
				for (int j = 0, max2 = leader.Count; j < max2; j++) {
					int leaderId = leader[j].leaderId;
					platoon.squads[leader[j].abreast].leader = DataTransfer.Convert(allLeaders[leaderId]);
					assignedLeader.Add(leaderId);
				}


				//分隊ごとにキャラIdを集める
				for (int j = 0; j < (int)Abreast.MAX; j++) {
					int fill = MstUnitData.GetData(platoon.squads[j].unitId).Fill;

					//小隊に所属するキャラ全体から分隊の列番号が一致するものを選抜して並び順にソートする
					var squad = units.Where(x => x.abreast == j).ToDictionary(mst=>mst.number);
					//squad.Sort((a, b) => a.number - b.number);
					platoon.squads[j].members = new List<CharacterData>();
					//並び終わったらIdだけ抜き出してリストにする。以降列番号、並び順は配列の要素番号から判断する。
					for (int k = 0; k < fill; k++) {
						//キャラが配置済
						if (squad.ContainsKey(k)) {
							int charaId = squad[k].charaId;
							platoon.squads[j].members.Add(DataTransfer.Convert(allCharacter[charaId]));
							assignedCharacter.Add(charaId);

						//キャラが配置されてない場合は無名キャラを設置
						} else {
							platoon.squads[j].members.Add(null);
						}
					}

				}
				platoons.Add(platoon);
			}
			platoonDict = platoons.ToDictionary(mst => mst.id);
			//全キャラクターから小隊に所属しているキャラIDを除外してリストを作る
			reserveCharas = DataTransfer.Convert(new List<UserMemberTable>(allCharacter.Values.Where(x => !assignedCharacter.Exists(c => c == x.id))));
			reserveLeaders = DataTransfer.Convert(new List<UserLeaderTable>(allLeaders.Values.Where(x => !assignedLeader.Exists(c => c == x.id))));

		}



		/// <summary>
		/// ステージのエネミー編成を成形
		/// </summary>
		public static PlatoonData LoadEnemyPlatoon(int enemyCorpsId) {
			MstEnemyCorps mst = BaseDataManager.GetDictionary<int, MstEnemyCorps>()[enemyCorpsId];
			return LoadEnemyPlatoon(mst);
		}

		/// <summary>
		/// ステージのエネミー編成を成形
		/// </summary>
		public static PlatoonData LoadEnemyPlatoon(MstEnemyCorps corps) {
			PlatoonData platoonData = new PlatoonData();
			platoonData.id = corps.Id;
			platoonData.squads = LoadEnemySquad(
				new int[]{corps.FirstUnit, corps.SecondUnit, corps.ThirdUnit},
				new int[]{corps.FirstLeader, corps.SecondLeader, corps.ThirdLeader});
			return platoonData;
		}
		/*/// <summary>
		/// リーダーの成形
		/// </summary>
		static LeaderData LoadLeader(int leaderId) {
			var mstLeader = MstEnemyLeaderData.GetData(leaderId);
			var mstEnemyParam = MstEnemyParam.GetData(mstLeader.EnemyParamId);

			LeaderData leader = new LeaderData();
			leader.id = leaderId;
			leader.leaderMasterId = mstEnemyParam.BaseId;

			return leader;
		}*/
		/// <summary>
		/// 一般ユニットの成形
		/// </summary>
		static List<SquadData> LoadEnemySquad(int[] unitId, int[] leaderId) {
			//MstEnemyParamはUserCharacterTable + MstUnitDataのような感じ
			//MstLeaderParamはUserLeaderTable + MstLeaderData + MstUnitDataのような感じ
			var enemyDict = BaseDataManager.GetDictionary<int, MstEnemyMemberParam>();
			List<MstEnemyMemberParam> enemieUnits = new List<MstEnemyMemberParam>();
			//1~3列目のユニットを設定
			for (int i = 0, max = unitId.Length; i < max; i++) {
				enemieUnits.Add(enemyDict[unitId[i]]);
			}

			var leaderDict = BaseDataManager.GetDictionary<int, MstEnemyLeaderParam>();
			List<MstEnemyLeaderParam> enemieLeaders = new List<MstEnemyLeaderParam>();
			//1~3列目のユニットを設定
			for (int i = 0, max = leaderId.Length; i < max; i++) {
				enemieLeaders.Add(leaderDict[leaderId[i]]);
			}


			var dict = BaseDataManager.GetDictionary<int, MstUnitData>();
			List<SquadData> groups = new List<SquadData>();
			for (int i = 0; i < enemieUnits.Count; i++) {
				SquadData squad = new SquadData();
				squad.unitId = enemieUnits[i].BaseId;
				squad.enemyMemberParamId = enemieUnits[i].Id;

				squad.members = new List<CharacterData>();
				for (int j = 0, max = dict[squad.unitId].Fill; j < max; j++) {
					CharacterData personal = new CharacterData();
					personal.name = "エネミー";
					personal.phase = UnityEngine.Random.Range(0, (int)PHASE.MAX);
					personal.generate = false;
					personal.portrait = "chara_0000";
					personal.ability = new List<int>();
					squad.members.Add(personal);
				}

				if (leaderId[i] != 0)
				{
					squad.leader = new LeaderData();
					squad.leader.leaderMasterId = enemieLeaders[i].BaseId;
					squad.leader.enemyLeaderParamId = enemieLeaders[i].Id;
				}
				groups.Add(squad);

			}

			return groups;
		}
	}


}
#endif
