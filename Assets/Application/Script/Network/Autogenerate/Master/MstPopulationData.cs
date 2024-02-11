using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstPopulationData {
		public int Id; // id
		public string DbgName; // 身分名
		public string Model; // モデル名
		public int Work; // 労働可能

		public static List<MstPopulationData> GetList() {
			return BaseDataManager.GetList<MstPopulationData>();
		}

		public static MstPopulationData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstPopulationData>().ContainsKey(id), typeof(MstPopulationData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstPopulationData>()[id];
		}
	}
}
