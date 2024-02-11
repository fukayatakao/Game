using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 何もしない状態
    /// </summary>
    public class StatePieceNone : IState<PieceEntity> {

        /// <summary>
        /// 状態開始時処理
        /// </summary>
        public override void Enter(PieceEntity owner) {
        }

        /// <summary>
        /// 状態終了時処理
        /// </summary>
        public override void Exit(PieceEntity owner) {
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Execute(PieceEntity owner) {
        }
    }
}
