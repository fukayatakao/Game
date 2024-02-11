using UnityEngine;
using Project.Lib;
using System.Collections.Generic;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SelectMenuUtil<T> {
		//選択キャラ
		int showMax = 8;
		int index_;
		List<T> selectable_;
		System.Func<T, string> captionFunc_;
		System.Action<T> selectAction_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SelectMenuUtil() {
		}

		public void Init(List<T> selectable, int max, System.Func<T, string> captionFunc, System.Action<T> selectAction) {
			selectable_ = selectable;
			showMax = max;
			selectAction_ = selectAction;
			captionFunc_ = captionFunc;
			index_ = 0;
		}

		public void Draw() {
			using (new GUILayout.HorizontalScope()) {
				//一番上にいる場合はボタン無効にする
				if (index_ == 0) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↑")) {
					index_--;
				}
				GUI.enabled = true;
				if (index_ >= selectable_.Count - showMax) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↓")) {
					index_++;
				}
				GUI.enabled = true;

				if (selectable_.Count > showMax) {
					index_ = Mathf.Clamp(index_, 0, selectable_.Count - showMax);
				}
			}
			int max = showMax < selectable_.Count ? showMax : selectable_.Count;
			for (int i = 0; i < max; i++) {
				T item = selectable_[index_ + i];
				if (FitGUILayout.Button(captionFunc_(item))) {
					selectAction_(item);
				}

			}
		}

	}

#endif
}
