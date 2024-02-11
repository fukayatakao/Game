using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateGachaMain : IState<GachaMain> {

		public override void Enter(GachaMain owner) {
			Gesture.Enable();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(GachaMain owner) {
			Gesture.Disable();
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(GachaMain owner) {
			//owner.ExecUserContorl();
		}


	}
}
