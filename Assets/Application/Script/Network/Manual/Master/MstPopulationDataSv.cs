using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public class MstPopulationDataSv {
		public MstPopulationDataSv(MstPopulationData origin, List<MstPopulationDemandData> list) {
			Id = origin.Id;
			Model = origin.Model;
			Name = origin.DbgName;
			Work = origin.Work;

			DemandArticle = new List<int>();

			for(int i =  0, max = list.Count; i < max; i++) {
				if(list[i].Population == Id) {
					DemandArticle.Add(list[i].Id);
				}
			}
		}

		public int Id; // charaId
		public string Name; // 身分名
		public string Model; // モデル名
		public int Work; // 労働可能
		public List<int> DemandArticle; // 需要
	}
}
