using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Collections.Generic;
//Linq重いから嫌い
using System.Linq;

namespace Project.Game {
	public static class PlayerAssetDownload {

		/// <summary>
		/// キャラクターで使用するアセットのアドレスを収集
		/// </summary>
		public static List<string> CreateAssetsList(List<SquadData> groups) {
			HashSet<string> list = new HashSet<string>();

			Dictionary<int, MstUnitData> unitDict = BaseDataManager.GetDictionary<int, MstUnitData>();
			Dictionary<int, MstLeaderData> leaderDict = BaseDataManager.GetDictionary<int, MstLeaderData>();

			//兵科からモデルのアセットアドレスを取得
			for (int i = 0; i < groups.Count; i++) {
				{
					string model = unitDict[groups[i].unitId].ModelName;
					list.Add(model);
				}
				{
					//リーダーからモデルのアセットアドレスを取得
					//列にリーダーがいなかったら無視
					if (groups[i].leader == null)
						continue;

					string model = leaderDict[groups[i].leader.leaderMasterId].UnitData.ModelName;
					list.Add(model);
				}

			}

			return list.ToList<string>();
		}

		/// <summary>
		/// キャラクターで使用するアセットのアドレスを収集
		/// </summary>
		public static List<string> CreateCategoryList(List<SquadData> groups) {
			HashSet<string> list = new HashSet<string>();

			Dictionary<int, MstUnitData> unitDict = BaseDataManager.GetDictionary<int, MstUnitData>();
			Dictionary<int, MstLeaderData> leaderDict = BaseDataManager.GetDictionary<int, MstLeaderData>();

			//兵科からモデルのアセットアドレスを取得
			for (int i = 0; i < groups.Count; i++) {
				{
					MstUnitData mst = unitDict[groups[i].unitId];
					list.Add(mst.AnimationLabel);
					list.Add(mst.ActionLabel);
				}
				//リーダーからモデルのアセットアドレスを取得
				{
					//列にリーダーがいなかったら無視
					if (groups[i].leader == null)
						continue;
					MstUnitData mst = leaderDict[groups[i].leader.leaderMasterId].UnitData;
					list.Add(mst.AnimationLabel);
					list.Add(mst.ActionLabel);
				}
			}



			list.Add(AddressableDefine.Label.AnimationCommon);


			return list.ToList<string>();

		}

	}
}
