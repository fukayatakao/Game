using UnityEngine;

namespace Project.Lib
{
#if DEVELOP_BUILD
	public static class FitGUI {
		/// <summary>
		/// ラベル表示
		/// </summary>
		public static void Label(Rect r, string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE) {
			GUI.skin.label.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.Label(FitCommon.CalcRectSize(r), text, GUI.skin.label);
		}
		/// <summary>
		/// ボタン表示
		/// </summary>
		public static void Button(Rect r, string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.Button(FitCommon.CalcRectSize(r), text, GUI.skin.button);
		}
		/// <summary>
		/// ボタン表示
		/// </summary>
		public static int SelectionGrid(Rect r, int selected, string[] text, int xCount, int fontSize = FitCommon.DEFAULT_FONT_SIZE) {
			GUI.skin.button.fontSize = FitCommon.CalcFontSize(fontSize);
			GUI.skin.toggle.fontSize = FitCommon.CalcFontSize(fontSize);
			return GUI.SelectionGrid(FitCommon.CalcRectSize(r), selected, text, xCount);
		}

		/// <summary>
		/// ボタン表示
		/// </summary>
		public static void Box(Rect r, string text, int fontSize = FitCommon.DEFAULT_FONT_SIZE) {
			GUI.Box(FitCommon.CalcRectSize(r), text);
		}

		/// <summary>
		/// テクスチャ表示
		/// </summary>
		public static void DrawTexture(Rect r, Texture2D tex) {
			GUI.DrawTexture(FitCommon.CalcRectSize(r), tex);
		}


	}
#endif

}
