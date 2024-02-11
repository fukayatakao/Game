using UnityEngine;
using Project.Account;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SceneSelectMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0f, 0f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.MiddleCenter;

		DebugMenuDrawer drawer = new DebugMenuDrawer(
			DebugMenu.Item("root",
				DebugMenu.Item("開始", () => {
					LoginManager.Login(()=>{SceneTransition.ChangeTown();});
				}),
				DebugMenu.Item("ユーザー初期化", () => {
					PlayerPrefs.DeleteAll();
					PlayerPrefsUtil.DeleteAll();
				})
			)
		);

		public SceneSelectMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}
		protected override void Draw(int id) {
			drawer.Draw();
		}
	}
#endif
}
