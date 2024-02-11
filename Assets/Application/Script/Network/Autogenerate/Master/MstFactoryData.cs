using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstFactoryData {
		public int Id; // id
		public string DbgName; // 施設名
		public string Model; // モデル名
		public int Goods; // 生産品1
		public int Output; // 生産量(/t)
		public int Goods2; // 生産品2
		public int Output2; // 生産量(/t)
		public int MinGrade; // 品質下限
		public int MaxGrade; // 品質上限
		public int MaxWorker; // 労働者数
		public float Range; // 効果範囲
		public int Cost; // 維持費
		public int PurchasePrice; // 購入費用

		public static List<MstFactoryData> GetList() {
			return BaseDataManager.GetList<MstFactoryData>();
		}

		public static MstFactoryData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstFactoryData>().ContainsKey(id), typeof(MstFactoryData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstFactoryData>()[id];
		}
	}
}
