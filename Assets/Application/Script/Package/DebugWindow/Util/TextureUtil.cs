using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public static class TextureUtil {
		//背景テクスチャ青
		private static Color colBlue = new Color(0f, 0f, 1f, 0.4f);
		//背景テクスチャ赤
		private static Color colRed = new Color(1f, 0f, 0f, 0.4f);
		//背景テクスチャグレー
		private static Color colGray = new Color(0.5f, 0.5f, 0.5f, 0.4f);

		//背景テクスチャ青
		public static Texture2D bgBlue = CreateColorTexture(colBlue);
		//背景テクスチャ赤
		public static Texture2D bgAlert = CreateColorTexture(new Color(1f, 0f, 0f, 0.7f));
		//背景テクスチャグレー
		public static Texture2D[] bgGray = new Texture2D[] { CreateColorTexture(colGray), CreateColorTexture(colGray + Color.black * 0.3f) };
		public static void TextureAlpha(float a) {
			colBlue.a = a;
			colRed.a = a;
			colGray.a = a;

			bgBlue = CreateColorTexture(colBlue);
			bgAlert = CreateColorTexture(colRed);
			bgGray = new Texture2D[] { CreateColorTexture(colGray), CreateColorTexture(colGray + Color.black * 0.3f) };
		}

		/// <summary>
		/// 指定した色のテクスチャを作成する
		/// </summary>
		public static Texture2D CreateColorTexture(Color color) {
			Texture2D createTexture = new Texture2D(1, 1);
			createTexture.SetPixel(0, 0, color);
			createTexture.Apply();
			return createTexture;
		}

	}

#endif

}
