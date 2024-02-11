using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstGeneratorData {
		public int Id; // id
		public string DbgName; // 品目名
		public string Model; // モデル名
		public int Price; // 価格
		public int Item; // 創出アイテム種別
		public int Point; // 創出量
		public int Cost; // 維持費
		public int PurchasePrice; // 購入費用

		public static List<MstGeneratorData> GetList() {
			return BaseDataManager.GetList<MstGeneratorData>();
		}

		public static MstGeneratorData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstGeneratorData>().ContainsKey(id), typeof(MstGeneratorData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstGeneratorData>()[id];
		}
	}
}
