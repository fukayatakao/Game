using System.Collections.Generic;

namespace Project.Game {
	public static class BattleAssetDownload {


		public static void CreateDownloadList(Project.Network.BattleSituationData data, out List<string> assets, out List<string> labels, out List<string> scene) {
			//アセット個別ロード
			assets = new List<string>();
			assets.AddRange(CommonAssetDownload.CreateAssetsList());
			assets.AddRange(PlayerAssetDownload.CreateAssetsList(data.invader.squads));
			assets.AddRange(PlayerAssetDownload.CreateAssetsList(data.defender.squads));

			//カテゴリ単位のロード
			labels = new List<string>();
			labels.AddRange(CommonAssetDownload.CreateCategoryList());
			labels.AddRange(PlayerAssetDownload.CreateCategoryList(data.invader.squads));
			labels.AddRange(PlayerAssetDownload.CreateCategoryList(data.defender.squads));

			//シーンのロード
			scene = new List<string>();
			var list = Mst.BaseDataManager.GetList<Mst.MstFieldData>();
			scene.Add(list[data.stageId].StageName);
		}

	}
}
