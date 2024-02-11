using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class HeaderMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.98f, 0.1f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;
		private Townhall owner_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HeaderMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(Townhall owner)
		{
			owner_ = owner;
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			using (new GUILayout.HorizontalScope())
			{
				FitGUILayout.Label("", 300, 0);
				FitGUILayout.Label("所持金:" + owner_.HaveParam.Gold);
				if (owner_.HaveParam.Logistics > owner_.Data.Logistics) {
					GUI.color = Color.red;
				}
				FitGUILayout.Label(string.Format("流通ポイント: {0} / {1}", owner_.HaveParam.Logistics, owner_.Data.Logistics));
				GUI.color = Color.white;
				if (FitGUILayout.Button(string.Format("Chain情報"))) {
					TownAlternativeMenu.I.OpenChainInfoMenu();
				}

			}
		}
	}
#endif
}
