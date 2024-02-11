using Project.Lib;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SelectCharacterMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		//選択キャラ
		int showMax = 8;
		int index_;
		List<int> selectableCharacter_;
		System.Action<int> update_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SelectCharacterMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(List<int> selectable, System.Action<int> update) {
			update_ = update;
			index_ = 0;


			selectableCharacter_ = selectable;
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label("キャラ選択");
			if (FitGUILayout.Button("割当解除")) {
				update_(-1);
				Hide();
			}

			using (new GUILayout.HorizontalScope()) {
				//一番上にいる場合はボタン無効にする
				if (index_ == 0) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↑")) {
					index_--;
				}
				GUI.enabled = true;
				if (index_ >= selectableCharacter_.Count - showMax) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↓")) {
					index_++;
				}
				GUI.enabled = true;

				if (selectableCharacter_.Count > showMax) {
					index_ = Mathf.Clamp(index_, 0, selectableCharacter_.Count - showMax);
				}
			}
			int max = showMax < selectableCharacter_.Count ? showMax : selectableCharacter_.Count;
			for(int i = 0; i < max; i++) {
				int charaId = selectableCharacter_[index_ + i];
				if (FitGUILayout.Button(PeopleData.I.Characters[charaId].name)) {
					update_(charaId);
					Hide();
				}

			}
		}

	}
#endif
}
