using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstSkillData {
		public int Id; // id
		public string DbgName; // 特性名
		public string ActionName; // アクション名
		public int Type; // 効果タイプ
		public int Value; // 効果量
		public int Consume; // 消費SP

		public static List<MstSkillData> GetList() {
			return BaseDataManager.GetList<MstSkillData>();
		}

		public static MstSkillData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstSkillData>().ContainsKey(id), typeof(MstSkillData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstSkillData>()[id];
		}
	}
}
