using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstLeaderData {
		public int Id; // id
		public string DbgName; // キャラクター名
		public int UnitId; // ユニットId
		public int SkillA; // スキルa
		public int SkillB; // スキルb
		public int SkillC; // スキルc
		public int SkillD; // スキルd

		public static List<MstLeaderData> GetList() {
			return BaseDataManager.GetList<MstLeaderData>();
		}

		public static MstLeaderData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstLeaderData>().ContainsKey(id), typeof(MstLeaderData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstLeaderData>()[id];
		}
	}
}
