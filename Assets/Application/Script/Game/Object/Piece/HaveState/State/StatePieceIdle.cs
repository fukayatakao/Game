using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 待機状態
	/// </summary>
	public class StatePieceIdle : IState<PieceEntity> {
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(PieceEntity owner) {
            owner.HaveModel.HaveAnimation.Play(BattleMotion.Idle);
		}

		/// <summary>
		/// 状態終了時処理
		/// </summary>
		public override void Exit(PieceEntity owner) {
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(PieceEntity owner){
            //@todo 仮対応
            if (!owner.HaveModel.HaveAnimation.IsPlay(BattleMotion.Idle))
                owner.HaveModel.HaveAnimation.Play(BattleMotion.Idle);
		}
	}
}
