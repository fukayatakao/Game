using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstResidenceData {
		public int Id; // id
		public string DbgName; // 施設名
		public string Model; // モデル名
		public int Population; // 身分id
		public int RoomCount; // 人数
		public float Range; // 効果範囲
		public int MaxGrade; // 最大生活水準
		public int Cost; // 維持費
		public int PurchasePrice; // 購入費用

		public static List<MstResidenceData> GetList() {
			return BaseDataManager.GetList<MstResidenceData>();
		}

		public static MstResidenceData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstResidenceData>().ContainsKey(id), typeof(MstResidenceData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstResidenceData>()[id];
		}
	}
}
