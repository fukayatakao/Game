using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 開始待ちステート
	/// </summary>
	public class StateBattleReady : IState<BattleMain> {


		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(BattleMain owner) {
#if DEVELOP_BUILD
			var menu = DebugWindowManager.Open<MainMenu>();
#endif
			Gesture.Enable();
			owner.GameReady();

#if UNITY_EDITOR
			if (owner as MockBattleMain)
			{
				BattleMessage.GameStart.Broadcast();
				menu.Hide();
			}
#endif
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(BattleMain owner) {
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(BattleMain owner) {
			Gesture.Disable();
		}


	}

}
