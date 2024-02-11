using Project.Mst;
using System.Collections.Generic;
using Project.Http.Mst;

namespace Project.Server {
	public static class InitialUserData {
		//初期ユニット
		static readonly int[] DefaultUnitId = new int[3] {2, 2, 2};
		//最大小隊数
		const int PLATOON_MAX = 4;


		/// <summary>
		/// デフォルトのデータを作る(所持アイテム)
		/// </summary>
		public static void CreateInitItem(out List<UserItemTable> itemDataTable)
		{
			itemDataTable = new List<UserItemTable>();

			//チュートリアルの順: 住居->農場->製粉->パン->マーケット

			itemDataTable.Add(new UserItemTable() { id = 1, type = (int)FacilityType.Factory, number = 1 });		//農場
			itemDataTable.Add(new UserItemTable() { id = 2, type = (int)FacilityType.Factory, number = 1 });		//製粉所
			itemDataTable.Add(new UserItemTable() { id = 3, type = (int)FacilityType.Factory, number = 1 });		//パン工房
			itemDataTable.Add(new UserItemTable() { id = 1, type = (int)FacilityType.Residence, number = 1 });		//住居
			itemDataTable.Add(new UserItemTable() { id = 1, type = (int)FacilityType.Market, number = 1 });			//マーケット
		}
		/// <summary>
		/// デフォルトのデータを作る(タウン)
		/// </summary>
		public static void CreateInitTown(out UserGlobalDataTable globalDataTable, out UserTownhallTable townhallTable)
		{
			globalDataTable = new UserGlobalDataTable();
			globalDataTable.id = 0;

			townhallTable = new UserTownhallTable();
			townhallTable.id = 0;
			townhallTable.baseId = 1;
			townhallTable.position = new UnityEngine.Vector3();
			townhallTable.tax = 5;
			townhallTable.gold = 1000;
			townhallTable.logistics = 0;
		}

		/// <summary>
		/// デフォルトのデータを作る(キャラクター)
		/// </summary>
		public static void CreateInitMember(out List<UserMemberTable> characters)
		{
			characters = new List<UserMemberTable>();
			//初期キャラはランダム３人、部隊配置なし
			characters.Add(LotteryTool.LotteryCharacter((int)SPECIES_TYPE.HUMAN));
			characters.Add(LotteryTool.LotteryCharacter((int)SPECIES_TYPE.HUMAN));
			characters.Add(LotteryTool.LotteryCharacter((int)SPECIES_TYPE.HUMAN));
		}

		/// <summary>
		/// デフォルトのデータを作る(バトル)
		/// </summary>
		public static void CreateInitCorps(out List<UserPlatoonTable> platoons, out List<UserPlatoonMemberTable> platoonUnits, out List<UserPlatoonLeaderTable> platoonLeaders, out List<UserLeaderTable> leaders) {
			platoons = new List<UserPlatoonTable>();
			platoonUnits = new List<UserPlatoonMemberTable>();
			leaders = new List<UserLeaderTable>();
			platoonLeaders = new List<UserPlatoonLeaderTable>();

			List<Mst.MstUnitData> unitData = new List<Mst.MstUnitData>() {
				BaseDataManager.GetDictionary<int, Mst.MstUnitData>()[DefaultUnitId[0]],
				BaseDataManager.GetDictionary<int, Mst.MstUnitData>()[DefaultUnitId[1]],
				BaseDataManager.GetDictionary<int, Mst.MstUnitData>()[DefaultUnitId[2]],
			};
			for (int i = 0; i < PLATOON_MAX; i++) {
				UserPlatoonTable platoon = new UserPlatoonTable();
				platoon.id = ++UserDB.Instance.platoonTableCounter;
				platoon.name = "部隊" + i.ToString();
				platoon.experience = GameConst.Battle.EXPERIENCE_SCALE * 3/ 4;			//最大値の75%の熟練度にしておく
				platoon.unitId1 = DefaultUnitId[0];
				platoon.unitId2 = DefaultUnitId[1];
				platoon.unitId3 = DefaultUnitId[2];

				platoons.Add(platoon);
			}

			//リーダーを追加
			leaders.Add(LotteryTool.LotteryLeader(1));
			leaders.Add(LotteryTool.LotteryLeader(4));
			leaders.Add(LotteryTool.LotteryLeader(5));
			leaders.Add(LotteryTool.LotteryLeader(6));
			leaders.Add(LotteryTool.LotteryLeader(9));
			platoonLeaders.Add(new UserPlatoonLeaderTable() { platoonId = platoons[0].id, abreast = (int)Abreast.Third, leaderId = leaders[0].id });

		}
		/// <summary>
		/// キャラクター作成
		/// </summary>
		private static List<UserMemberTable> CreateInitCharacters(List<Mst.MstUnitData> unitData) {
			List<UserMemberTable> charas = new List<UserMemberTable>();
			//まずはキャラを定員数作る
			foreach(var data in unitData) {
				for (int i = 0, max = data.Fill; i < max; i++) {
					charas.Add(LotteryTool.LotteryCharacter(data.Species));
				}
			}
			return charas;
		}

		/// <summary>
		/// キャラクターを部隊にアサイン
		/// </summary>
		private static List<UserPlatoonMemberTable> AssignPlatoon(List<UserMemberTable> charas, int platoonId, List<Mst.MstUnitData> unitData) {
			List<UserPlatoonMemberTable> platoonUnits = new List<UserPlatoonMemberTable>();
			//確保したキャラの所属を割り当て
			int count = 0;
			for (int i = 0; i < (int)Abreast.MAX; i++) {
				for (int j = 0; j < unitData[i].Fill; j++) {
					var pc = new UserPlatoonMemberTable();
					pc.platoonId = platoonId;
					pc.abreast = i;
					pc.number = j;
					pc.charaId = charas[count].id;
					platoonUnits.Add(pc);
					count++;
				}
			}
			return platoonUnits;
		}

	}
}
