using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstVersion {
		public int Id; // 
		public string Name; // テーブル名
		public string Value; // ハッシュ値

		public static List<MstVersion> GetList() {
			return BaseDataManager.GetList<MstVersion>();
		}

		public static MstVersion GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstVersion>().ContainsKey(id), typeof(MstVersion).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstVersion>()[id];
		}
	}
}
