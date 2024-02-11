using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// マテリアル処理クラス
	/// </summary>
	public class SkinRenderer : MonoPretender {
		private List<SkinnedMeshRenderer> skinRendererList_ = new List<SkinnedMeshRenderer>();
		/// <summary>
		/// 初期化
		/// </summary>
		public void Cache(Transform cacheTrans)
		{
			skinRendererList_.Clear ();
			// 子オブジェクトを含めた全Renderer取得
			skinRendererList_.AddRange(cacheTrans.GetComponentsInChildren<SkinnedMeshRenderer> ());
		}
		/// <summary>
		/// 画面外に出た時にアニメーションを止めるかどうか
		/// </summary>
		public void SetRendererUpdateWhenOffscreen(bool isEnable) {
			for (int i = 0, max = skinRendererList_.Count; i < max; i++) {
				skinRendererList_ [i].updateWhenOffscreen = isEnable;
			}
		}


	}
}
