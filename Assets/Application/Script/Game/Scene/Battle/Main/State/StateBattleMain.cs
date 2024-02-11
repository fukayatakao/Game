using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateBattleMain : IState<BattleMain> {
		public override void Enter(BattleMain owner) {
			Gesture.Enable();
            BattleCollision.Enable();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(BattleMain owner) {
			BattleCollision.Disable();
			Gesture.Disable();
        }
        /// <summary>
        /// 更新
        /// </summary>
		public override void Execute(BattleMain owner) {
			//一番最初に制御チェックを行う
			owner.ExecUserContorl();
		}


	}
}
