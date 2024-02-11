using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化前の何もしない状態
	/// </summary>
	public class StateBattleNone : IState<BattleMain> {

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(BattleMain owner) {
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
		}



	}

}
