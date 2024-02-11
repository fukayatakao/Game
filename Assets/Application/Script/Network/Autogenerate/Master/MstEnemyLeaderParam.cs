using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstEnemyLeaderParam {
		public int Id; // id
		public string DbgName; // キャラクター名
		public int BaseId; // ユニットId
		public int MaxHp; // 最大HP
		public int MaxLp; // 最大LP
		public int Attack; // 攻撃力
		public int Defence; // 防御力
		public int SkillA; // スキルa
		public int SkillB; // スキルb
		public int SkillC; // スキルc
		public int SkillD; // スキルd

		public static List<MstEnemyLeaderParam> GetList() {
			return BaseDataManager.GetList<MstEnemyLeaderParam>();
		}

		public static MstEnemyLeaderParam GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstEnemyLeaderParam>().ContainsKey(id), typeof(MstEnemyLeaderParam).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstEnemyLeaderParam>()[id];
		}
	}
}
