using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstTownhallData {
		public int Id; // id
		public string DbgName; // 施設名
		public string Model; // モデル名
		public int Cost; // 維持費
		public int Logistics; // 購入費用
		public float Range; // 効果範囲

		public static List<MstTownhallData> GetList() {
			return BaseDataManager.GetList<MstTownhallData>();
		}

		public static MstTownhallData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstTownhallData>().ContainsKey(id), typeof(MstTownhallData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstTownhallData>()[id];
		}
	}
}
