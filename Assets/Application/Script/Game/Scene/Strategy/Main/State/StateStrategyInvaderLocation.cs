using Project.Lib;


namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateStrategyInvaderLocation : IState<StrategyMain> {
		private SelectNode selectNode_;


		public override void Enter(StrategyMain owner) {
			Gesture.Enable();

			selectNode_ = new SelectNode(owner.MainCamera);
			selectNode_.Register();

		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
			selectNode_.UnRegister();
			Gesture.Disable();
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(StrategyMain owner) {
			selectNode_.Execute();
			//一番最初に制御チェックを行う
			owner.ExecUserContorl();
		}

	}
}
