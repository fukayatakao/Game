using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class MainMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.MiddleCenter;


		MessageSystem.Receptor receptor_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainMenu() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent);
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			//CloseButton.Draw(this);
			if (FitGUILayout.Button("GameStart")) {

				BattleMessage.GameStart.Broadcast();
				Hide();
			}



		}
	}
#endif
}
