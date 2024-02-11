using System.Collections.Generic;

namespace Project.Mst {
	public partial class MstLeaderData {
		public void Resolve(Dictionary<int, MstUnitData> unitDict) {
			UnitData = unitDict[UnitId];
		}

		public MstUnitData UnitData; // ユニットデータの参照
	}
}
