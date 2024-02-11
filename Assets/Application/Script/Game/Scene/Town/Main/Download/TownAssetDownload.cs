using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;

//Linq重いから嫌い

namespace Project.Game {
	public static class TownAssetDownload {


		public static void CreateDownloadList(TownMainResponse data, out List<string> assets, out List<string> labels, out List<string> scene) {
			//アセット個別ロード
			assets = new List<string>();
			assets.AddRange(CreateAssetsList());

			//カテゴリ単位のロード
			labels = new List<string>();
			labels.AddRange(CreateCategoriesList());

			//シーンのロード
			scene = new List<string>();
			scene.AddRange(CreateScenesList());
		}


		private static List<string> CreateAssetsList() {
			List<string> assets = new List<string>();

			assets.Add(AddressableDefine.Address.CIRCLE_AREA);

			{
				//生産施設モデル
				var list = BaseDataManager.GetList<MstFactoryData>();
				for(int i = 0, max = list.Count; i < max; i++) {
					if (string.IsNullOrEmpty(list[i].Model))
						continue;
					assets.Add(list[i].Model);
				}
			}
			{
				//倉庫モデル
				var list = BaseDataManager.GetList<MstStorageData>();
				for (int i = 0, max = list.Count; i < max; i++) {
					if (string.IsNullOrEmpty(list[i].Model))
						continue;
					assets.Add(list[i].Model);
				}
			}
			{
				//マーケットモデル
				var list = BaseDataManager.GetList<MstMarketData>();
				for (int i = 0, max = list.Count; i < max; i++) {
					if (string.IsNullOrEmpty(list[i].Model))
						continue;
					assets.Add(list[i].Model);
				}
			}
			{
				//住居モデル
				var list = BaseDataManager.GetList<MstResidenceData>();
				for (int i = 0, max = list.Count; i < max; i++) {
					if (string.IsNullOrEmpty(list[i].Model))
						continue;
					assets.Add(list[i].Model);
				}
			}

			return assets;
		}

		private static List<string> CreateCategoriesList() {
			List<string> labels = new List<string>();
			labels.Add("Animation-Normal");
			labels.Add(AddressableDefine.Label.AnimationCommon);

			return labels;
		}

		private static List<string> CreateScenesList() {
			List<string> scene = new List<string>();
			scene.Add("Field/Ground");
			return scene;
		}

	}
}
