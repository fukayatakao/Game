using UnityEngine;

namespace Project.Lib
{
#if DEVELOP_BUILD
	public static class FitGUILayout {
		/// <summary>
		/// ラベル表示
		/// </summary>
		public static void Label(string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.label.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.label.alignment = anchor;
			GUI.skin.label.fontStyle = FontStyle.Normal;
			GUI.skin.label.wordWrap = false;
			GUILayout.Label(text, GUI.skin.label);
		}

		/// <summary>
		/// ラベル表示
		/// </summary>
		/// <remarks>
		/// デフォルトサイズの場合はw,hに0を指定
		/// </remarks>
		public static void Label(string text, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.label.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.label.alignment = anchor;
			GUI.skin.label.fontStyle = FontStyle.Normal;
			GUI.skin.label.wordWrap = false;
			if (w == 0 && h == 0) {
				GUILayout.Label(text, GUI.skin.label);
			} else if (w == 0) {
				GUILayout.Label(text, GUI.skin.label, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				GUILayout.Label(text, GUI.skin.label, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				GUILayout.Label(text, GUI.skin.label, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}

		/// <summary>
		/// ボタン表示
		/// </summary>
		public static bool Button(string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleCenter) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.button.alignment = anchor;
			GUI.skin.button.fontStyle = FontStyle.Normal;
			return GUILayout.Button(text, GUI.skin.button);
		}
		/// <summary>
		/// ボタン表示
		/// </summary>
		/// <remarks>
		/// デフォルトサイズの場合はw,hに0を指定
		/// </remarks>
		public static bool Button(string text, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleCenter) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.button.alignment = anchor;
			GUI.skin.button.fontStyle = FontStyle.Normal;
			if (w == 0 && h == 0) {
				return GUILayout.Button(text, GUI.skin.button);
			} else if (w == 0) {
				return GUILayout.Button(text, GUI.skin.button, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				return GUILayout.Button(text, GUI.skin.button, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				return GUILayout.Button(text, GUI.skin.button, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}
		/// <summary>
		/// 選択ボタン表示
		/// </summary>
		public static int SelectionGrid(int selected, string[] text, int xCount, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleCenter) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.button.alignment = anchor;
			GUI.skin.button.fontStyle = FontStyle.Normal;

			GUI.skin.toggle.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.toggle.fontStyle = FontStyle.Normal;

			return GUILayout.SelectionGrid(selected, text, xCount);
		}

		/// <summary>
		/// 選択ボタン表示
		/// </summary>
		public static int SelectionGrid(int selected, string[] text, int xCount, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleCenter) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.button.alignment = anchor;
			GUI.skin.button.fontStyle = FontStyle.Normal;
			if (w == 0 && h == 0) {
				return GUILayout.SelectionGrid(selected, text, xCount);
			} else if (w == 0) {
				return GUILayout.SelectionGrid(selected, text, xCount, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				return GUILayout.SelectionGrid(selected, text, xCount, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				return GUILayout.SelectionGrid(selected, text, xCount, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}

		/// <summary>
		/// スクロールビュー開始
		/// </summary>
		public static Vector2 BeginScrollView(Vector2 scrollPos, float barSize = 15f) {
			GUI.skin.verticalScrollbar.fixedWidth = (int)FitCommon.CalcWidth(barSize);
			GUI.skin.verticalScrollbarThumb.fixedWidth = (int)FitCommon.CalcWidth(barSize);

			GUI.skin.horizontalScrollbar.fixedHeight = (int)FitCommon.CalcHeight(barSize);
			GUI.skin.horizontalScrollbarThumb.fixedHeight = (int)FitCommon.CalcHeight(barSize);
			return GUILayout.BeginScrollView(scrollPos);
		}

		/// <summary>
		/// スクロールビュー終了
		/// </summary>
		public static void EndScrollView() {
			GUILayout.EndScrollView();
		}

		/// <summary>
		/// Toggleボタン
		/// </summary>
		public static bool Toggle(bool value, string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.toggle.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.toggle.alignment = anchor;
			GUI.skin.toggle.fontStyle = FontStyle.Normal;
			return GUILayout.Toggle(value, text, GUI.skin.toggle);
		}

		/// <summary>
		/// Toggleボタン
		/// </summary>
		/// <remarks>
		/// デフォルトサイズの場合はw,hに0を指定
		/// </remarks>
		public static bool Toggle(bool value, string text, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.toggle.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.toggle.alignment = anchor;
			GUI.skin.toggle.fontStyle = FontStyle.Normal;
			if (w == 0 && h == 0) {
				return GUILayout.Toggle(value, text, GUI.skin.toggle);
			} else if (w == 0) {
				return GUILayout.Toggle(value, text, GUI.skin.toggle, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				return GUILayout.Toggle(value, text, GUI.skin.toggle, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				return GUILayout.Toggle(value, text, GUI.skin.toggle, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}

		/// <summary>
		/// Sliderバー
		/// </summary>
		public static float Slider(float value, float min, float max, int thumbSize = 24) {
			GUI.skin.horizontalSlider.fixedHeight = FitCommon.CalcFontSize(thumbSize);
			GUI.skin.horizontalSliderThumb.fixedHeight = FitCommon.CalcFontSize(thumbSize);
			GUI.skin.horizontalSliderThumb.fixedWidth = FitCommon.CalcFontSize(thumbSize / 3);
			return GUILayout.HorizontalSlider(value, min, max);
		}

		/// <summary>
		/// Sliderバー
		/// </summary>
		/// <remarks>
		/// デフォルトサイズの場合はw,hに0を指定
		/// </remarks>
		public static float Slider(float value, float min, float max, int w, int h, int thumbSize = 24) {
			GUI.skin.horizontalSlider.fixedHeight = FitCommon.CalcFontSize(thumbSize);
			GUI.skin.horizontalSliderThumb.fixedHeight = FitCommon.CalcFontSize(thumbSize);
			GUI.skin.horizontalSliderThumb.fixedWidth = FitCommon.CalcFontSize(thumbSize / 3);

			if (w == 0 && h == 0) {
				return GUILayout.HorizontalSlider(value, min, max);
			} else if (w == 0) {
				return GUILayout.HorizontalSlider(value, min, max, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				return GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				return GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}


		/// <summary>
		/// Text入力
		/// </summary>
		public static string TextField(string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.textField.fontSize = FitCommon.CalcFontSize(fontSize);
			return GUILayout.TextField(text, 32);
		}

		/// <summary>
		/// Text入力
		/// </summary>
		public static string TextField(string text, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.textField.fontSize = FitCommon.CalcFontSize(fontSize);
			return GUILayout.TextField(text, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
		}

		/// <summary>
		/// Textエリア
		/// </summary>
		public static string TextArea(string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.textArea.fontSize = FitCommon.CalcFontSize(fontSize);
			return GUILayout.TextArea(text);
		}

		/// <summary>
		/// Textエリア
		/// </summary>
		/// <remarks>
		/// デフォルトサイズの場合はw,hに0を指定
		/// </remarks>
		public static string TextArea(string text, int w, int h, int fontSize = FitCommon.DEFAULT_FONT_SIZE, TextAnchor anchor = TextAnchor.MiddleLeft) {
			GUI.skin.textArea.fontSize = FitCommon.CalcFontSize(fontSize);

			if (w == 0 && h == 0) {
				return GUILayout.TextArea(text);
			} else if (w == 0) {
				return GUILayout.TextArea(text, GUILayout.Height(FitCommon.CalcHeight(h)));
			} else if (h == 0) {
				return GUILayout.TextArea(text, GUILayout.Width(FitCommon.CalcWidth(w)));
			} else {
				return GUILayout.TextArea(text, GUILayout.Width(FitCommon.CalcWidth(w)), GUILayout.Height(FitCommon.CalcHeight(h)));
			}
		}
	}
#endif

}
