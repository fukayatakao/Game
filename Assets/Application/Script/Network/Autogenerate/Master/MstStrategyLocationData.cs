using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstStrategyLocationData {
		public int Id; // id
		public string DbgName; // デバッグ名
		public int Map; // マップID
		public int Node; // ノードID
		public int Platoon; // 部隊ID

		public static List<MstStrategyLocationData> GetList() {
			return BaseDataManager.GetList<MstStrategyLocationData>();
		}

		public static MstStrategyLocationData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstStrategyLocationData>().ContainsKey(id), typeof(MstStrategyLocationData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstStrategyLocationData>()[id];
		}
	}
}
