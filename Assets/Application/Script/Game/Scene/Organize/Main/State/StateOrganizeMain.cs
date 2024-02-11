using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateOrganizeMain : IState<OrganizeMain> {

		public override void Enter(OrganizeMain owner) {
			Gesture.Enable();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(OrganizeMain owner) {
			Gesture.Disable();
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(OrganizeMain owner) {
			owner.ExecUserContorl();
		}


	}
}
