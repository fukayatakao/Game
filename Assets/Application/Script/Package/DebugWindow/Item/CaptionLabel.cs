using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
    /// <summary>
    /// キャプション表示
    /// </summary>
	public class CaptionLabel {
        public static void Draw(string text) {
            FitGUILayout.Label(text, FitCommon.SMALL_FONT_SIZE, TextAnchor.MiddleCenter);
		}
	}

#endif
}


