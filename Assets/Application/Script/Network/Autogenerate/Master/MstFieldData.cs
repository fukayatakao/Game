using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstFieldData {
		public int Id; // id
		public string StageName; // 
		public string SkyName; // 
		public int Width; // ステージ横幅
		public int Depth; // ステージ奥行
		public int BackDepth; // 両端陣地の奥行

		public static List<MstFieldData> GetList() {
			return BaseDataManager.GetList<MstFieldData>();
		}

		public static MstFieldData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstFieldData>().ContainsKey(id), typeof(MstFieldData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstFieldData>()[id];
		}
	}
}
