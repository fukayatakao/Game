using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SwapMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.15f, 0.25f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerRight;

		private PlatoonEntity owner_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SwapMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		public void Init(PlatoonEntity owner) {
			owner_ = owner;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			//交代できないときは押せないようにする
			if (owner_.IsDisableForeSwap()) {
				GUI.enabled = false;
			}
			if (FitGUILayout.Button("ForeSwitch")) {
				BattleMessage.SquadForeSwap.Broadcast(Power.Player);
			}
			GUI.enabled = true;

			//交代できないときは押せないようにする
			if (owner_.IsDisableAftSwap()) {
				GUI.enabled = false;
			}
			if (FitGUILayout.Button("AftSwitch")) {
				BattleMessage.SquadAftSwap.Broadcast(Power.Player);
			}
			GUI.enabled = true;

			//交代できないときは押せないようにする
			if (owner_.IsDisableRotation()) {
				GUI.enabled = false;
			}
			if (FitGUILayout.Button("Rotation")) {
				BattleMessage.SquadRotation.Broadcast(Power.Player);
			}
			GUI.enabled = true;

			GUI.enabled = true;
		}
	}
#endif
}
