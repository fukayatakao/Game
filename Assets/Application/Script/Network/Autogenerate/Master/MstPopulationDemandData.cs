using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstPopulationDemandData {
		public int Id; // id
		public string DbgName; // 需要名
		public int Population; // 身分id
		public int Article; // 品目id
		public int Number; // 必要種別数
		public int Amount; // 消費量
		public int Category; // 需要分類(0:必需 1:贅沢)

		public static List<MstPopulationDemandData> GetList() {
			return BaseDataManager.GetList<MstPopulationDemandData>();
		}

		public static MstPopulationDemandData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstPopulationDemandData>().ContainsKey(id), typeof(MstPopulationDemandData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstPopulationDemandData>()[id];
		}
	}
}
