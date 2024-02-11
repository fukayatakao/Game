using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化前の何もしない状態
	/// </summary>
	public class StateGachaNone : IState<GachaMain> {

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(GachaMain owner) {
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(GachaMain owner) {
		}

		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(GachaMain owner) {
		}



	}

}
