using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstEnemyMemberParam {
		public int Id; // id
		public string DbgName; // キャラクター名
		public int BaseId; // ベースユニット
		public int MaxHp; // 最大HP
		public int MaxLp; // 最大LP
		public int Attack; // 攻撃力
		public int Defence; // 防御力
		public int Ability1; // 抽選可能アビリティ1
		public int Ability2; // 抽選可能アビリティ2
		public int Ability3; // 抽選可能アビリティ3

		public static List<MstEnemyMemberParam> GetList() {
			return BaseDataManager.GetList<MstEnemyMemberParam>();
		}

		public static MstEnemyMemberParam GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstEnemyMemberParam>().ContainsKey(id), typeof(MstEnemyMemberParam).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstEnemyMemberParam>()[id];
		}
	}
}
