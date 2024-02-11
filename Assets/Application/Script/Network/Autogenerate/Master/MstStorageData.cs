using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstStorageData {
		public int Id; // id
		public string DbgName; // 施設名
		public string Model; // モデル名
		public int Capacity; // 容量
		public int Volume; // 容積
		public float Range; // 効果範囲
		public int Cost; // 維持費
		public int PurchasePrice; // 購入費用

		public static List<MstStorageData> GetList() {
			return BaseDataManager.GetList<MstStorageData>();
		}

		public static MstStorageData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstStorageData>().ContainsKey(id), typeof(MstStorageData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstStorageData>()[id];
		}
	}
}
