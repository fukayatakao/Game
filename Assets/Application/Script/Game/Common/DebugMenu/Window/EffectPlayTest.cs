using UnityEngine;
using Project.Game;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class EffectPlayTest : DebugWindow {
		static readonly Rect DefaultWindowRect = FitCommon.CalcRect(new Vector2(0.01f, 0.01f), new Vector2(0.3f, 0.2f), FitCommon.Alignment.UpperRight);
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EffectPlayTest() {
			base.Init(DefaultWindowRect);
			SetAutoResize();



			ShowEvent = () => { Time.timeScale = 1f; };
			HideEvent = () => { Time.timeScale = 0f; };

		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			if (FitGUILayout.Button("再生")) {
				EffectAssembly.I.CreateAsync("Shock", (s) => { s.SetLifeTime(2f); });
			}
		}

	}
#endif
}
