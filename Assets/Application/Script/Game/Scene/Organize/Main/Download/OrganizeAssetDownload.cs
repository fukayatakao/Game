using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
//Linq重いから嫌い
using System.Linq;

namespace Project.Game {
	public static class OrganizeAssetDownload {

		public static void CreateDownloadList(OrganizeSituationData data, out List<string> assets, out List<string> labels, out List<string> scene) {
			//アセット個別ロード
			assets = new List<string>();
			for(int i = 0, max = data.platoons.Count; i < max; i++) {
				assets.AddRange(CreateDownloadList(data.platoons[i]));
			}

			//カテゴリ単位のロード
			labels = new List<string>();
			for (int i = 0, max = data.platoons.Count; i < max; i++) {
				labels.AddRange(CollectCharacterCategory(data.platoons[i]));
			}
			//シーンのロード
			scene = new List<string>();
			scene.Add(Mst.MstFieldData.GetData(data.stageId).StageName);
		}


		/// <summary>
		/// ダウンロードリスト作成
		/// </summary>
		private static List<string> CreateDownloadList(PlatoonData data) {
			//ダウンロードリスト作成
			List<string> downloadList = new List<string>();
			downloadList.AddRange(CollectCharacterAsset(data));

			downloadList.Add("Character/AI/basic");
			downloadList.Add("Platoon/AI/basic");
			downloadList.Add("Character/AI/edit");
			downloadList.Add(AddressableDefine.Address.CIRCLE_AREA);


			return downloadList;
		}

		/// <summary>
		/// キャラクターで使用するアセットのアドレスを収集
		/// </summary>
		private static List<string> CollectCharacterAsset(PlatoonData data) {
			HashSet<string> list = new HashSet<string>();

			Dictionary<int, MstUnitData> charaDict = BaseDataManager.GetDictionary<int, MstUnitData>();

			//兵科からモデルのアセットアドレスを取得
			/*for (int i = 0;i < data.groups.Count; i++) {
				int charaId = data.groups[i].DefaultUnitId;


				list.Add(charaDict[charaId].Model);
			}

			//リーダーからモデルのアセットアドレスを取得
			for (int i = 0; i < data.leaders.Count; i++) {
				int charaId = data.leaders[i].DefaultUnitId;
				list.Add(charaDict[charaId].Model);
			}*/


			//@todo 仮で全部のアニメーション・アクションをロードする
			List<MstUnitData> mstList = BaseDataManager.GetList<MstUnitData>();
			for (int i = 1, max = mstList.Count; i < max; i++) {
				list.Add(mstList[i].ModelName);
			}



			list.Add("Effect/Shock");
			list.Add("Effect/Empty");

			return list.ToList<string>();
		}

		/// <summary>
		/// キャラクターで使用するアセットのアドレスを収集
		/// </summary>
		private static List<string> CollectCharacterCategory(PlatoonData data) {
			HashSet<string> list = new HashSet<string>();

			Dictionary<int, MstUnitData> charaDict = BaseDataManager.GetDictionary<int, MstUnitData>();

			//兵科からモデルのアセットアドレスを取得
			for (int i = 0; i < data.squads.Count; i++) {
				var chara = charaDict[data.squads[i].unitId];
				list.Add(chara.AnimationLabel);
				list.Add(chara.ActionLabel);
				//リーダーからモデルのアセットアドレスを取得
				{
					if (data.squads[i].leader == null)
						continue;
					var leader = charaDict[data.squads[i].leader.leaderMasterId];
					list.Add(leader.AnimationLabel);
					list.Add(leader.ActionLabel);
				}
			}


			//@todo 仮で全部のアニメーション・アクションをロードする
			List<MstUnitData> mstList = BaseDataManager.GetList<MstUnitData>();
			for (int i = 1, max = mstList.Count; i < max; i++) {
				list.Add(mstList[i].ActionLabel);
				list.Add(mstList[i].AnimationLabel);
			}



			list.Add(AddressableDefine.Label.AnimationCommon);


			return list.ToList<string>();

		}

	}
}
