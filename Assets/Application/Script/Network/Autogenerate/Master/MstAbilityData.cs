using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstAbilityData {
		public int Id; // id
		public string DbgName; // 特性名
		public int Group; // グループ
		public int Rarity; // レアリティ
		public int Type; // 効果タイプ
		public int Value; // 効果量

		public static List<MstAbilityData> GetList() {
			return BaseDataManager.GetList<MstAbilityData>();
		}

		public static MstAbilityData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstAbilityData>().ContainsKey(id), typeof(MstAbilityData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstAbilityData>()[id];
		}
	}
}
