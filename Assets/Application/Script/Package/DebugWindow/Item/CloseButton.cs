using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// ウィンドウの閉じるボタン
	/// </summary>
	public class CloseButton {
        private static Rect rect_ = new Rect(8f, 8f, 48f, 48f);

        /// <summary>
        /// UI表示
        /// </summary>
        public static void Draw(DebugWindow owner) {

            Rect r = FitCommon.CalcRectSize(rect_);
            r.x = owner.WindowRect.width - r.x - r.width;
			if(GUI.Button(r, "×")) {
                owner.Hide();
            }
		}
	}
	/// <summary>
	/// デバッグウィンドウ全閉じボタン
	/// </summary>
	public class ExitButton {
        private static Rect rect_ = new Rect(8f, 8f, 48f, 48f);

        /// <summary>
        /// UI表示
        /// </summary>
        public static void Draw(DebugWindow owner) {

            Rect r = FitCommon.CalcRectSize(rect_);
            r.x = owner.WindowRect.width - r.x - r.width;
			if(GUI.Button(r, "×")) {
				DebugWindowManager.Exit();
            }
		}
	}
#endif
}


