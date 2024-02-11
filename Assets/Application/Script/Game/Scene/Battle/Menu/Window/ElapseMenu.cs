using UnityEngine;
using Project.Lib;
using Project.Http.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class ElapseMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		BattlePhaseBonus bonus_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ElapseMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(BattlePhaseBonus bonus) {
			bonus_ = bonus;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			GUILayout.BeginHorizontal();
			FitGUILayout.Label("時間帯ボーナス", 240, 0);
			FitGUILayout.Label("", 64, 64);
			GUI.DrawTexture(GUILayoutUtility.GetLastRect(), ResourcePool.GetPhaseIcon((PHASE)bonus_.current));
			GUILayout.EndHorizontal();
		}
	}
#endif
}
