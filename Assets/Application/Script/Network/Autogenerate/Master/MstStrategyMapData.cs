using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Mst {
	[System.Serializable]
	public partial class MstStrategyMapData {
		public int Id; // id
		public string DbgName; // デバッグ名
		public string SceneName; // シーンアセット名
		public string WaymapName; // 経路マップ
		public int Location; // 配置情報
		public int MaxTurn; // 最大ターン

		public static List<MstStrategyMapData> GetList() {
			return BaseDataManager.GetList<MstStrategyMapData>();
		}

		public static MstStrategyMapData GetData(int id) {
			Debug.Assert(BaseDataManager.GetDictionary<int, MstStrategyMapData>().ContainsKey(id), typeof(MstStrategyMapData).Name + " not found id:" + id);
			return BaseDataManager.GetDictionary<int, MstStrategyMapData>()[id];
		}
	}
}
