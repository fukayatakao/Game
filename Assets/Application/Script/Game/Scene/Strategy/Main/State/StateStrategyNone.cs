using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化前の何もしない状態
	/// </summary>
	public class StateStrategyNone : IState<StrategyMain> {

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(StrategyMain owner) {
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(StrategyMain owner) {
		}

		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
		}



	}

}
