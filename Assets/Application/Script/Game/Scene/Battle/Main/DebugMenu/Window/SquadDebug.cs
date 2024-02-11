using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class SquadDebug : DebugWindow {
		Vector2 scrollPos_ = new Vector2();
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;
		int teamIndex_ = 0;
		static readonly string[] TeamLabel = new string[] { "Player", "Enemy" };
		static readonly string[] IndexLabel = System.Enum.GetNames(typeof(Abreast));
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SquadDebug() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			CaptionLabel.Draw("列削除");
			scrollPos_ = FitGUILayout.BeginScrollView(scrollPos_);


			//チーム選択
			teamIndex_ = Selector.Draw(teamIndex_, TeamLabel);

			if (FitGUILayout.Button(TeamLabel[teamIndex_] + ":" + IndexLabel[0])) {
				KillUnit((Power)teamIndex_, Abreast.First);
			}
			if (FitGUILayout.Button(TeamLabel[teamIndex_] + ":" + IndexLabel[1])) {
				KillUnit((Power)teamIndex_, Abreast.Second);
			}
			if (FitGUILayout.Button(TeamLabel[teamIndex_] + ":" + IndexLabel[2])) {
				KillUnit((Power)teamIndex_, Abreast.Third);
			}

			FitGUILayout.EndScrollView();
		}

		private void KillUnit(Power team, Abreast group) {
			for(int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterEntity entity = CharacterAssembly.I.Current[i];
				if(entity.Platoon.Index == team && entity.Squad.Index == group) {
					entity.HaveUnitParam.Fight.Lp = 0f;
					entity.Dead();
				}
			}
		}

	}
#endif
}
