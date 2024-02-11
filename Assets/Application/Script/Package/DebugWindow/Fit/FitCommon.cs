using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public static class FitCommon {
		public const int DEFAULT_FONT_SIZE = 12 * 3;
		public const int SMALL_FONT_SIZE = 10 * 3;
		public const int LARGE_FONT_SIZE = 14 * 3;

		/// <summary>
		/// フォントを実ピクセルのサイズに換算
		/// </summary>
		public static int CalcFontSize(int fontSize) {
			return (int)(fontSize * VirtualScreen.Screen2Pixel.y);
		}
		/// <summary>
		/// 矩形サイズを実ピクセルのサイズに換算
		/// </summary>
		public static Rect CalcRectSize(Rect rect) {
			Rect r = new Rect();
			r.x = rect.x * VirtualScreen.Screen2Pixel.x;
			r.y = rect.y * VirtualScreen.Screen2Pixel.y;
			r.width = rect.width * VirtualScreen.Screen2Pixel.x;
			r.height = rect.height * VirtualScreen.Screen2Pixel.y;

			return r;
		}
		/// <summary>
		/// 矩形サイズを実ピクセルのサイズに換算
		/// </summary>
		public static Rect CalcScreenRectSize(Rect rect) {
			Rect r = new Rect();
			r.x = rect.x * VirtualScreen.Pixel2Screen.x;
			r.y = rect.y * VirtualScreen.Pixel2Screen.y;
			r.width = rect.width * VirtualScreen.Pixel2Screen.x;
			r.height = rect.height * VirtualScreen.Pixel2Screen.y;

			return r;
		}

		/// <summary>
		/// 横幅を実ピクセルのサイズに換算
		/// </summary>
		public static float CalcWidth(float width) {
			return width * VirtualScreen.Screen2Pixel.x;
		}
		/// <summary>
		/// 縦幅を実ピクセルのサイズに換算
		/// </summary>
		public static float CalcHeight(float height) {
			return height * VirtualScreen.Screen2Pixel.y;
		}
		/// <summary>
		/// ウィンドウの初期位置
		/// </summary>
		public enum Alignment : int {
			UpperLeft,
			UpperCenter,
			UpperRight,
			MiddleLeft,
			MiddleCenter,
			UMiddleRight,
			LowerLeft,
			LowerCenter,
			LowerRight,
		}
		/// <summary>
		/// 設定値からウィンドウの矩形を計算
		/// </summary>
		public static Rect CalcRect(Vector2 mergin, Vector2 size, Alignment align) {
			Rect r = new Rect();
			r.width = size.x * VirtualScreen.ScreenWidth;
			r.height = size.y * VirtualScreen.ScreenHeight;
			//水平方向の揃え
			switch ((int)align % 3) {
				//左揃え
				case 0: r.x = mergin.x * VirtualScreen.ScreenWidth; break;
				//中央揃え
				case 1: r.x = (1f - size.x) * 0.5f * VirtualScreen.ScreenWidth; break;
				//右揃え
				case 2: r.x = (1f - size.x - mergin.x) * VirtualScreen.ScreenWidth; break;
			}

			//垂直方向の揃え
			switch ((int)align / 3) {
				//上揃え
				case 0: r.y = mergin.y * VirtualScreen.ScreenHeight; break;
				//中央揃え
				case 1: r.y = (1f - size.y) * 0.5f * VirtualScreen.ScreenHeight; break;
				//下揃え
				case 2: r.y = (1f - mergin.y - size.y) * VirtualScreen.ScreenHeight; break;
			}

			if (r.x < 0f)
				r.x = 0f;
			if (r.y < 0f)
				r.y = 0f;
			//if (r.x + r.width > VirtualScreenUtil.ScreenWidth)
			//	r.width = VirtualScreenUtil.ScreenWidth - r.x;

			return r;
		}

#if UNITY_EDITOR
		static Vector2 screenSize_ = Vector2.zero;
		/// <summary>
		/// Unityでゲームウィンドウのサイズを変更したか
		/// </summary>
		/// <returns></returns>
		public static bool IsResize() {
			bool result = (screenSize_.x != VirtualScreen.Screen2Pixel.x || screenSize_.y != VirtualScreen.Screen2Pixel.y);
			screenSize_ = VirtualScreen.Screen2Pixel;
			return result;
		}
#endif
	}

#endif

}
