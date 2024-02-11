using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstUnitData {
		public int Id; // id
		public string DbgName; // ユニット名
		public string ModelName; // モデル名
		public string ActionLabel; // アクション
		public string AnimationLabel; // アニメーション
		public string AiName; // AIファイル
		public int Species; // 種族
		public int Fill; // 員数
		public int MaxHp; // 最大HP
		public int MaxLp; // 最大LP
		public int Attack; // 攻撃力
		public int Defence; // 防御力
		public float AimSpeedDeg; // 旋回速度(deg/s)
		public float Recovery; // 回復速度(%/s)
		public int Phase; // 属性値
		public int Pircing; // 貫通
		public int Armor; // 装甲
		public int Stagger; // 被ノックバック距離(m)
		public float MaxSpeed; // 移動速度(m/s)
		public float Accelerate; // 加速度(m/s2)
		public float SearchRange; // 索敵距離(m)
		public int ActionA; // 攻撃方法a
		public int ActionB; // 攻撃方法b
		public int ActionC; // 攻撃方法c
		public int ActionD; // 攻撃方法d
		public int Forward; // 前衛適性

		public static List<MstUnitData> GetList() {
			return BaseDataManager.GetList<MstUnitData>();
		}

		public static MstUnitData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstUnitData>().ContainsKey(id), typeof(MstUnitData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstUnitData>()[id];
		}
	}
}
