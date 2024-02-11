using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 何もしない状態
    /// </summary>
    public class StateCharacterNone : IState<CharacterEntity> {

        /// <summary>
        /// 状態開始時処理
        /// </summary>
        public override void Enter(CharacterEntity owner) {
        }

        /// <summary>
        /// 状態終了時処理
        /// </summary>
        public override void Exit(CharacterEntity owner) {
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Execute(CharacterEntity owner) {
        }
    }
}
