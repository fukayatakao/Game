using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class DisplayTime : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0f, 0f);
		static readonly Vector2 size = new Vector2(0.1f, 0.1f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperCenter;


		MessageSystem.Receptor receptor_;
		private BattleTimer timer_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DisplayTime() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent);
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		public void Init(BattleTimer timer)
		{
			timer_ = timer;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			//CloseButton.Draw(this);
			GUILayout.BeginHorizontal();
			FitGUILayout.Label(timer_.PastTime.ToString("00"));
			FitGUILayout.Label("/");
			FitGUILayout.Label(timer_.LimitTime.ToString("00"));
			GUILayout.EndHorizontal();
		}
	}
#endif
}
