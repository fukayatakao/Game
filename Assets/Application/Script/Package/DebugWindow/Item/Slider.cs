using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
    /// <summary>
    /// スライダー表示
    /// </summary>
	public class Slider {
        public static float Draw(string text, float val, float min, float max) {
            FitGUILayout.Label(string.Format("{0}:{1:0.00}", text, val));
            val = FitGUILayout.Slider(val, min, max);

			return val;
		}

		/// <summary>
		/// 微調整用のボタン付き
		/// </summary>
        public static float Draw(string text, float val, float min, float max, float delta) {
            FitGUILayout.Label(string.Format("{0}:{1:0.00}", text, val));
            GUILayout.BeginHorizontal();
            if (FitGUILayout.Button("<", FitCommon.DEFAULT_FONT_SIZE, 0, (int)(FitCommon.DEFAULT_FONT_SIZE * 0.7f))) {
                val -= delta;
            }
            val = FitGUILayout.Slider(val, min, max);
            if (FitGUILayout.Button(">", FitCommon.DEFAULT_FONT_SIZE, 0, (int)(FitCommon.DEFAULT_FONT_SIZE * 0.7f))) {
                val += delta;
            }
            GUILayout.EndHorizontal();

            return val;
        }
	}

#endif
}


