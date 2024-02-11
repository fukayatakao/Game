using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstMarketData {
		public int Id; // id
		public string DbgName; // 施設名
		public string Model; // モデル名
		public int Capacity; // 容量
		public int Article; // 取扱品目ID
		public float Range; // 効果範囲
		public int Cost; // 維持費
		public int PurchasePrice; // 購入費用

		public static List<MstMarketData> GetList() {
			return BaseDataManager.GetList<MstMarketData>();
		}

		public static MstMarketData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstMarketData>().ContainsKey(id), typeof(MstMarketData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstMarketData>()[id];
		}
	}
}
