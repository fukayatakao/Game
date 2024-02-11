using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// スキル演出中状態
	/// </summary>
	public class StateBattleSpecial : IState<BattleMain>
	{
		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(BattleMain owner) {
			owner.HaveTimer.Stop();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(BattleMain owner) {
			owner.HaveTimer.Resume();
        }
        /// <summary>
        /// 更新
        /// </summary>
		public override void Execute(BattleMain owner) {
		}


	}
}
