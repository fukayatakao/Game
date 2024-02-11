using UnityEngine;

namespace Project.Lib {
	public class FieldFog : MonoBehaviour {
		public int mode;			// フォグモード
		public Color color;			// フォグカラー
		public float density;		// フォグ密度指数
		public int startDistance;	// フォグ発生距離
		public int endDistance;		// フォグ終了距離

		private void Start () {
			ApplyFog();
		}

		/// <summary>
		/// 削除時
		/// ライトマップの参照を破棄
		/// </summary>
		void OnDestroy() {
			RenderSettings.fog = false;
		}

		/// <summary>
		/// フォグをセットします
		/// </summary>
		private void ApplyFog () {
			RenderSettings.fog = true;
			RenderSettings.fogMode = (FogMode)mode;
			RenderSettings.fogColor =  color;
			RenderSettings.fogDensity = density;
			RenderSettings.fogStartDistance = startDistance;
			RenderSettings.fogEndDistance = endDistance;
		}
	}
}
