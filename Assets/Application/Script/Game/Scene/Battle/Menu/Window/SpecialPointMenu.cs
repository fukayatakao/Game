using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SpecialPointMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.3f, 0.1f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		static Texture2D tex1 = TextureUtil.CreateColorTexture(new Color(0.1f, 0.9f, 0.1f));
		static Texture2D tex2 = TextureUtil.CreateColorTexture(new Color(0.1f, 0.5f, 0.1f));
		static Texture2D tex3 = TextureUtil.CreateColorTexture(new Color(0.1f, 0.3f, 0.1f));

		BattleMain main_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SpecialPointMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(BattleMain main) {
			main_ = main;
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			FitGUILayout.Label("SPゲージ");

			PlatoonEntity player = main_.Platoon[(int)Power.Player];
			float mergin_y = 0.08f;
			float mergin_x = 0.05f;
			float h = 0.25f;

			int point = player.HaveSpecialPoint.Point;
			//point += (int)(GameConst.Battle.SPECIAL_POINT_CONSUME * 1.5f);

			float r1 = (float)Mathf.Clamp(point, 0, GameConst.Battle.SPECIAL_POINT_CONSUME) / GameConst.Battle.SPECIAL_POINT_CONSUME;
			float r2 = (float)Mathf.Clamp(point - GameConst.Battle.SPECIAL_POINT_CONSUME, 0, GameConst.Battle.SPECIAL_POINT_CONSUME) / GameConst.Battle.SPECIAL_POINT_CONSUME;
			float r3 = (float)Mathf.Clamp(point - GameConst.Battle.SPECIAL_POINT_CONSUME * 2, 0, GameConst.Battle.SPECIAL_POINT_CONSUME) / GameConst.Battle.SPECIAL_POINT_CONSUME;

			float y = WindowRect.height * mergin_y;
			GUI.DrawTexture(new Rect(WindowRect.width * mergin_x, y, WindowRect.width * (1f - mergin_x * 2) * r1, WindowRect.height * h), tex1);
			y += WindowRect.height * h;
			GUI.DrawTexture(new Rect(WindowRect.width * mergin_x, y, WindowRect.width * (1f - mergin_x * 2) * r2, WindowRect.height * h), tex2);
			y += WindowRect.height * h;
			GUI.DrawTexture(new Rect(WindowRect.width * mergin_x, y, WindowRect.width * (1f - mergin_x * 2) * r3, WindowRect.height * h), tex3);
			//GUI.DrawTexture(new Rect(WindowRect.width * mergin_x, WindowRect.height * (mergin_y + size.y), WindowRect.width * (size.x * lpRatio), WindowRect.height * size.y), lpTex);

		}
	}
#endif
}
