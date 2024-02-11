using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstGoodsData {
		public int Id; // id
		public string DbgName; // 品名
		public string Model; // モデル名
		public int Article; // 品目ID
		public int Price; // 価格
		public int ResourceId1; // 必要品1
		public int UseAmount1; // 消費量
		public int ResourceId2; // 必要品2
		public int UseAmount2; // 消費量
		public int ResourceId3; // 必要品3
		public int UseAmount3; // 消費量

		public static List<MstGoodsData> GetList() {
			return BaseDataManager.GetList<MstGoodsData>();
		}

		public static MstGoodsData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstGoodsData>().ContainsKey(id), typeof(MstGoodsData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstGoodsData>()[id];
		}
	}
}
