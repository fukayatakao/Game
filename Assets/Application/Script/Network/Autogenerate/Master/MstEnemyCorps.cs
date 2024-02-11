using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstEnemyCorps {
		public int Id; // id
		public string DbgName; // 名称
		public int FirstUnit; // 前列ユニット種別
		public int FirstLeader; // リーダーパラメータ
		public int SecondUnit; // 中列パラメータ
		public int SecondLeader; // リーダーパラメータ
		public int ThirdUnit; // 後列パラメータ
		public int ThirdLeader; // リーダーパラメータ
		public string Ai; // AIファイル名

		public static List<MstEnemyCorps> GetList() {
			return BaseDataManager.GetList<MstEnemyCorps>();
		}

		public static MstEnemyCorps GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstEnemyCorps>().ContainsKey(id), typeof(MstEnemyCorps).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstEnemyCorps>()[id];
		}
	}
}
