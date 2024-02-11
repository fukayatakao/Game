using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstQuestData {
		public int Id; // id
		public string DbgName; // デバッグ名
		public int Map; // マップID
		public int Platoon; // 部隊ID

		public static List<MstQuestData> GetList() {
			return BaseDataManager.GetList<MstQuestData>();
		}

		public static MstQuestData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstQuestData>().ContainsKey(id), typeof(MstQuestData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstQuestData>()[id];
		}
	}
}
