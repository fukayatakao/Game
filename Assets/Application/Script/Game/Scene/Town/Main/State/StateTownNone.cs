using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化前の何もしない状態
	/// </summary>
	public class StateTownNone : IState<TownMain> {

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(TownMain owner) {
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(TownMain owner) {
		}

		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(TownMain owner) {
		}



	}

}
