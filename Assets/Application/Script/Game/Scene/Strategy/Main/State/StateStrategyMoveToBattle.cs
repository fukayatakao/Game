using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateStrategyMoveToBattle : IState<StrategyMain> {
		CoroutineTaskList taskList_ = new CoroutineTaskList();
		MessageSystem.Receptor receptor_;

		public void Init() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
		}

		/// <summary>
		/// 開始
		/// </summary>
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
