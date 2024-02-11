using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class TransitMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.1f);
		private static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerRight;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TransitMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			if (FitGUILayout.Button("部隊編成")) {
				SceneTransition.ChangeOrganize(SceneTransition.SceneType.Town);
			}
			if (FitGUILayout.Button("ガチャ")) {
				SceneTransition.ChangeGacha();
			}
			if (FitGUILayout.Button("クエスト")) {
				SceneTransition.ChangeQuestBattle(1, 1);
			}
			if (FitGUILayout.Button("攻略戦")) {
				SceneTransition.ChangeStrategy();
			}

		}
	}
#endif
}
