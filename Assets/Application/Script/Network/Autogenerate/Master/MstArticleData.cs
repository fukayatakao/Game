using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstArticleData {
		public int Id; // id
		public string DbgName; // 品目名
		public string Model; // モデル名
		public int Price; // 価格

		public static List<MstArticleData> GetList() {
			return BaseDataManager.GetList<MstArticleData>();
		}

		public static MstArticleData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstArticleData>().ContainsKey(id), typeof(MstArticleData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstArticleData>()[id];
		}
	}
}
