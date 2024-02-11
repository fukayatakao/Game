using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class BattleEndMessage : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0f, 0f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.MiddleCenter;


		MessageSystem.Receptor receptor_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BattleEndMessage() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent);
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			//CloseButton.Draw(this);
			if (FitGUILayout.Button("Finish")) {
				if (BattleMain.PrevScene == SceneTransition.SceneType.Town) {
					SceneTransition.ChangeTown();
				} else {
					int invaderId = BattleSituation.I.invader.id;
					int defenderId = BattleSituation.I.defender.id;
					int nodeId = BattleSituation.I.nodeId;
					SceneTransition.ChangeStrategyWin(invaderId, defenderId, nodeId);
				}
			}
		}
	}
#endif
}
