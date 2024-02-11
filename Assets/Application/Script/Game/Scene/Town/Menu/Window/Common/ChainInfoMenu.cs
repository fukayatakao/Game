using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class ChainInfoMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.4f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerLeft;
		private TownMain owner_;

		public ChainInfoMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(TownMain ower) {
			owner_ = ower;
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label("チェイン");
			foreach (var factory in owner_.Factories) {
				foreach (Chain chain in factory.SendList.Values) {
					if (!chain.Valid)
						continue;
					GUILayout.BeginHorizontal();
					if (FitGUILayout.Button("〇")) {
						owner_.MainCamera.LookAtView(chain.Sender.GetPosition());
					}
					FitGUILayout.Label(chain.Sender.Name);
					FitGUILayout.Label("->");
					if (FitGUILayout.Button("〇")) {
						owner_.MainCamera.LookAtView(chain.Receiver.GetPosition());
					}
					FitGUILayout.Label(chain.Receiver.Name);
					GUILayout.EndHorizontal();
				}
			}


		}

	}

#endif
}
