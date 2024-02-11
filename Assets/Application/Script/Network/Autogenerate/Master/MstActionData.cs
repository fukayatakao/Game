using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstActionData {
		public int Id; // id
		public string DbgName; // デバッグ名
		public string ActionName; // アクション名
		public string HitEffect; // ヒットエフェクト名
		public float RangeMax; // 最大攻撃可能距離(m)
		public float RangeMin; // 最小攻撃可能距離(m)
		public float AngleDeg; // 攻撃可能前方範囲(deg)
		public bool Indirect; // 味方越しに攻撃可能か
		public float ConsumeHp; // 消費HP
		public float Interval; // 攻撃頻度(s)
		public float Damage; // ダメージ倍率
		public float Knockback; // ノックバック量倍率
		public int Impact; // 与ノックバック蓄積
		public int Priority; // 優先度
		public string Check; // 特殊条件

		public static List<MstActionData> GetList() {
			return BaseDataManager.GetList<MstActionData>();
		}

		public static MstActionData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstActionData>().ContainsKey(id), typeof(MstActionData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstActionData>()[id];
		}
	}
}
