using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class OrganizeSystemMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.15f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerRight;

		private bool displayName_;
		OrganizeMain owner_;
		PlatoonEntity team_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrganizeSystemMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(OrganizeMain owner) {
			owner_ = owner;
			team_ = owner_.Platoon;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			FitGUILayout.Label("メニュー");

			if (FitGUILayout.Button("キャラクター名表示")) {
				displayName_ = !displayName_;
				OrganizeMessage.DisplayCharacterName.Broadcast(displayName_);
			}
			if (FitGUILayout.Button("更新")) {
				OrganizeMessage.UpdateCorps.Broadcast();
			}
			if (FitGUILayout.Button("戻る")) {
				if (OrganizeMain.PrevScene == SceneTransition.SceneType.Strategy) {
					SceneTransition.ChangeStrategy();
				} else {
					SceneTransition.ChangeTown();
				}
			}
		}
	}
#endif
}
