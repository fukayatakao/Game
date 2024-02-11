using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
    /// <summary>
    /// 切り替え選択
    /// </summary>
	public class Selector {
        public static int Draw(int index, string[] text) {
            using (new GUILayout.HorizontalScope()) {
                //前へ
                if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
                    index--;
                    if (index < 0)
                        index = text.Length - 1;
                }
                //現在選択しているものを表示
                FitGUILayout.Label(text[index], FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

                //次へ
                if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
                    index++;
                    if (index >= text.Length)
                        index = 0;
                }
            }

            return index;
        }
		public static int Draw(int index, int min, int max, int add, int accele_add, System.Func<int, string> label) {
			using (new GUILayout.HorizontalScope()) {
				//前へ
				if (FitGUILayout.Button("<<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= accele_add;
					if (index < min)
						index = min;
				}
				//前へ
				if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= add;
					if (index < min)
						index = min;
				}
				//現在選択しているものを表示
				FitGUILayout.Label(label(index), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

				//次へ
				if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += add;
					if (index > max)
						index = max;
				}
				//次へ
				if (FitGUILayout.Button(">>", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += accele_add;
					if (index > max)
						index = max;
				}
			}

			return index;
		}
		public static int Draw(int index, int min, int max, int add=1) {
            using (new GUILayout.HorizontalScope()) {
                //前へ
                if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
                    index -= add;
                    if (index < min)
                        index = min;
                }
                //現在選択しているものを表示
                FitGUILayout.Label(index.ToString(), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

                //次へ
                if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
                    index += add;
                    if (index > max)
                        index = max;
                }
            }

            return index;
        }
		public static int Draw(string label, int index, int min, int max, int add = 1) {
			using (new GUILayout.HorizontalScope()) {
				FitGUILayout.Label(label, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);
				//前へ
				if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= add;
					if (index < min)
						index = min;
				}
				//現在選択しているものを表示
				FitGUILayout.Label(index.ToString(), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

				//次へ
				if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += add;
					if (index > max)
						index = max;
				}
			}

			return index;
		}

		public static float Draw(float index, float min, float max, float add=1f) {
			using (new GUILayout.HorizontalScope()) {
				//前へ
				if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= add;
					if (index < min)
						index = min;
				}
				//現在選択しているものを表示
				FitGUILayout.Label(index.ToString(), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

				//次へ
				if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += add;
					if (index > max)
						index = max;
				}
			}

			return index;
		}
		public static int Draw(int index, int min, int max, int add, int accele_add) {
			using (new GUILayout.HorizontalScope()) {
				//前へ
				if (FitGUILayout.Button("<<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= accele_add;
					if (index < min)
						index = min;
				}
				//前へ
				if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= add;
					if (index < min)
						index = min;
				}
				//現在選択しているものを表示
				FitGUILayout.Label(index.ToString(), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

				//次へ
				if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += add;
					if (index > max)
						index = max;
				}
				//次へ
				if (FitGUILayout.Button(">>", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += accele_add;
					if (index > max)
						index = max;
				}
			}

			return index;
		}
		public static float Draw(float index, float min, float max, float add, float accele_add) {
			using (new GUILayout.HorizontalScope()) {
				//前へ
				if (FitGUILayout.Button("<<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= accele_add;
					if (index < min)
						index = min;
				}
				//前へ
				if (FitGUILayout.Button("<", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index -= add;
					if (index < min)
						index = min;
				}
				//現在選択しているものを表示
				FitGUILayout.Label(index.ToString("F"), FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter);

				//次へ
				if (FitGUILayout.Button(">", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += add;
					if (index > max)
						index = max;
				}
				//次へ
				if (FitGUILayout.Button(">>", 48, 0, FitCommon.DEFAULT_FONT_SIZE, TextAnchor.MiddleCenter)) {
					index += accele_add;
					if (index > max)
						index = max;
				}
			}

			return index;
		}
	}

#endif
}


