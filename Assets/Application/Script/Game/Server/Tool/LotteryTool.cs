using Project.Mst;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Server {
	public static class LotteryTool {
		const int MAX_ABILITY = 3;
		const int DefaultLeaderId = 1000;





		/// <summary>
		/// キャラクターを抽選
		/// </summary>
		public static UserMemberTable LotteryCharacter(int species) {
			UserMemberTable chara = new UserMemberTable();
			//名前はとりあえずランダム
			int index = UnityEngine.Random.Range(0, ConstantData.CharaName.Count - 1);
			chara.id = ++UserDB.Instance.memberTableCounter;
			chara.name = ConstantData.CharaName[index];
			chara.species = species;
			chara.generate = false;
			chara.portrait = LotteryPortrait(chara.id, chara.species);
			int[] ab = LotteryAbility();
			chara.ability1 = ab[0];
			chara.ability2 = ab[1];
			chara.ability3 = ab[2];
			chara.phase = LotteryPhase();

			//タウンの住居と職場の割り当てなし
			chara.stayId = -1;
			chara.staySlotId = -1;
			chara.workId = -1;
			chara.workSlotId = -1;

			return chara;
		}
		/// <summary>
		/// キャラクターを抽選
		/// </summary>
		public static UserLeaderTable LotteryLeader(int Id = -1) {
			UserLeaderTable leader = new UserLeaderTable();

			List<MstLeaderData> list = MstLeaderData.GetList();
			//指定がない場合はランダムで選ぶ
			if (Id == -1) {
				int index = UnityEngine.Random.Range(0, list.Count - 1);
				Id = list[index].Id;
			}
			leader.id = ++UserDB.Instance.leaderTableCounter;
			leader.leaderMasterId = Id;
			leader.limitbreak = 0;
			leader.lv = 1;
			leader.rarity = 1;

			return leader;
		}

		/// <summary>
		/// キャラ画像を抽選
		/// </summary>
		private static string LotteryPortrait(int id, int species) {
			//@todo キャラアイコン数は暫定
			int max = 0;
			switch ((Project.Http.Mst.SPECIES_TYPE)species) {
			case Http.Mst.SPECIES_TYPE.NONE:
				return "000";
			case Http.Mst.SPECIES_TYPE.HUMAN:
				max = 30;
				break;
			default:
				Debug.LogError("unknown type:" + species);
				break;
			}

			return "chara_" + (id % max).ToString("0000");
		}

		/// <summary>
		/// 特性を抽選する
		/// </summary>
		public static int[] LotteryAbility() {
			int[] ability = new int[MAX_ABILITY];

			//良特性、悪特性が偏りすぎないように仕様を考える
			List<MstAbilityData> list = Mst.BaseDataManager.GetList<MstAbilityData>();
			int count = UnityEngine.Random.Range(1, 2);
			for (int c = 0; c < count; c++) {
				int index = UnityEngine.Random.Range(1, list.Count - 1);
				ability[c] = list[index].Id;
			}


			return ability;
		}
		/// <summary>
		/// 属性を抽選する
		/// </summary>
		public static int LotteryPhase() {
			//とりあえず単純ランダム
			return Random.Range(0, (int)Project.Http.Mst.PHASE.MAX);
		}
	}
}
