using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
	//@note LightmapSettings.lightmapsを手動で制御する用。
	//OnDestroyでライトマップ操作されるとタイミング的に制御が難しい
	public class FieldLightMap : MonoBehaviour {

		[System.Serializable]
		public class LightMap {
			public Texture2D near = null;
			public Texture2D far = null;
		}

		[SerializeField]
		private List<LightMap> m_lightMapList = null;


		// ライトマップの適用
		public void Setup() {
			if (m_lightMapList.Count == 0) return;

			LightmapData[] lightmaps = new LightmapData[m_lightMapList.Count];

			for (int i = 0; i < m_lightMapList.Count; i++) {
				LightMap lightMap = m_lightMapList[i];

				LightmapData lightmap = new LightmapData();
				lightmap.lightmapDir = lightMap.near;
				lightmap.lightmapColor = lightMap.far;

				lightmaps[i] = lightmap;
			}

			LightmapSettings.lightmaps = lightmaps;
		}

		/// <summary>
		/// 削除時
		/// ライトマップの参照を破棄
		/// </summary>
		public void Cleanup() {
			LightmapSettings.lightmaps = null;
			m_lightMapList.Clear();
		}

	}
}
