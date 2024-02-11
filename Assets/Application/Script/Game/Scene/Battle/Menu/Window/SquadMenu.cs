using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SquadMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		static Texture2D lpTex = TextureUtil.CreateColorTexture(new Color(0.9f, 0.1f, 0.1f));
		static Texture2D hpTex = TextureUtil.CreateColorTexture(new Color(0.1f, 0.9f, 0.1f));

		private static Texture2D[] bgTexture = new Texture2D[]
		{
			TextureUtil.CreateColorTexture(new Color(0.3f, 0f, 0f, 0.7f)),
			TextureUtil.CreateColorTexture(new Color(0f, 0.3f, 0f, 0.7f)),
			TextureUtil.CreateColorTexture(new Color(0f, 0f, 0.3f, 0.7f)),
			TextureUtil.CreateColorTexture(new Color(0.3f, 0.3f, 0.3f, 0.7f)),
		};


		BattleMain main_;
		//操作する分隊
		int teamIndex_;
		int groupIndex_;
		static readonly string[] SelectLabel = new string[] { "1st", "2nd", "3rd", "boss" };
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SquadMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(BattleMain main, int teamIndex, int groupIndex) {
			main_ = main;
			teamIndex_ = teamIndex;
			groupIndex_ = groupIndex;
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			if (main_.Platoon[teamIndex_].Squads.Count - 1 < groupIndex_)
				return;

			SquadEntity squad = main_.Platoon[teamIndex_].Squads[groupIndex_];

			FitGUILayout.Label(SelectLabel[(int)squad.Index]);

			float mergin_y = 0.2f;
			float mergin_x = 0.15f;
			Vector2 size = new Vector2(0.8f, 0.2f);

			float lpRatio = squad.HaveParam.Lp / squad.HaveParam.MaxLp;
			float hpRatio = squad.HaveParam.Hp / squad.HaveParam.MaxHp;

			float left = WindowRect.width * mergin_x;
			GUI.DrawTexture(new Rect(left, WindowRect.height * mergin_y, WindowRect.width * (size.x * hpRatio), WindowRect.height * size.y), hpTex);
			GUI.DrawTexture(new Rect(left, WindowRect.height * (mergin_y + size.y), WindowRect.width * (size.x * lpRatio), WindowRect.height * size.y), lpTex);

			float top = WindowRect.height * (mergin_y + size.y * 2);
			GUI.DrawTexture(new Rect(left, top, 24, 24), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.SUN));
			GUI.Label(new Rect(left + 32, top, WindowRect.width * (size.x * lpRatio), 24f), squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.SUN).ToString());

			left += 56;
			GUI.DrawTexture(new Rect(left, top, 24, 24), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.MOON));
			GUI.Label(new Rect(left + 32, top, WindowRect.width * (size.x * lpRatio), 24f), squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.MOON).ToString());

			left += 56;
			GUI.DrawTexture(new Rect(left, top, 24, 24), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.STAR));
			GUI.Label(new Rect(left + 32, top, WindowRect.width * (size.x * lpRatio), 24f), squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.STAR).ToString());

			SetWindowTexture(bgTexture[(int)squad.Index], bgTexture[(int)squad.Index]);


		}
	}
#endif
}
