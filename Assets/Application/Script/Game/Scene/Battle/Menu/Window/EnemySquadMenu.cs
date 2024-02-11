using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class EnemySquadMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.21f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		BattleMain main_;
		//操作する分隊
		int teamIndex_;
		static string[] SelectLabel;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EnemySquadMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(BattleMain main) {
			main_ = main;
			teamIndex_ = (int)Power.Enemy;
			SelectLabel = FieldAttribute.GetFields(typeof(Abreast));
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {

			GUILayout.BeginHorizontal();
			for(int i = 0, max = main_.Platoon[teamIndex_].Squads.Count; i < max; i++) {
				SquadEntity squad = main_.Platoon[teamIndex_].Squads[i];
				FitGUILayout.Label(SelectLabel[i]);

				GUILayout.BeginVertical();
				using (var h = new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("", 24, 24);
					GUI.DrawTexture(GUILayoutUtility.GetLastRect(), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.SUN));
					FitGUILayout.Label(squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.SUN).ToString());
				}
				using (var h = new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("", 24, 24);
					GUI.DrawTexture(GUILayoutUtility.GetLastRect(), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.MOON));
					FitGUILayout.Label(squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.MOON).ToString());
				}
				using (var h = new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("", 24, 24);
					GUI.DrawTexture(GUILayoutUtility.GetLastRect(), ResourcePool.GetPhaseIcon(Http.Mst.PHASE.STAR));
					FitGUILayout.Label(squad.HaveParam.GetPhaseCount(Http.Mst.PHASE.STAR).ToString());
				}

				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

		}
	}
#endif
}
