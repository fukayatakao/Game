using Project.Mst;
using System.Collections.Generic;

namespace Project.Server {
	public class DemandData {
		public int Article;         //品目
		public int Number;          //個数
		public int Amount;          //量
		public DemandCategory Category;

		public DemandData Clone() {
			return (DemandData)MemberwiseClone();
		}
	}
	public static class PopTool {
		//populationId, 需要カテゴリ, 需要データ
		public static Dictionary<int, List<DemandData>> Demand;
		//residenceBaseId, 需要カテゴリ, 需要データ
		public static Dictionary<int, List<DemandData>> DemandByResidence;


		public static void Init() {
			//popごとの需要をリストにして検索しやすくする
			Demand = new Dictionary<int, List<DemandData>>();
			var mst = BaseDataManager.GetList<MstPopulationDemandData>();
			for (int i = 0, max = mst.Count; i < max; i++) {
				int key = mst[i].Population;
				DemandData data = new DemandData() {
					Article = mst[i].Article,
					Number = mst[i].Number,
					Amount = mst[i].Amount,
					Category = (DemandCategory)mst[i].Category,
				};
				if (!Demand.ContainsKey(key)) {
					Demand[key] = new List<DemandData>();
				}
				Demand[key].Add(data);
			}

			//residenceから探せるようにDictionaryを整理
			DemandByResidence = new Dictionary<int, List<DemandData>>();
			var residences = BaseDataManager.GetList<MstResidenceData>();
			for (int i = 0, max = residences.Count; i < max; i++) {
				//住居に住むpopの種別から需要を取得、住居BaseIdから探せるようにする
				int pop = residences[i].Population;
				DemandByResidence[residences[i].Id] = Demand[pop];
			}
		}

	}
}
