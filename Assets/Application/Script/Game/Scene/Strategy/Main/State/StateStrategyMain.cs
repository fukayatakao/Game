using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateStrategyMain : IState<StrategyMain> {
		public override void Enter(StrategyMain owner) {
			Gesture.Enable();

			//owner.MainCamera.ChangeControlView();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
			Gesture.Disable();
        }
        /// <summary>
        /// 更新
        /// </summary>
		public override void Execute(StrategyMain owner) {
			//一番最初に制御チェックを行う
			owner.ExecUserContorl();
		}


	}
}
