using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 移動状態
    /// </summary>
    public class StateCharacterMove : IState<CharacterEntity> {
		CharacterMove move_ = new CharacterMove();
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
			move_.Init(owner, owner.HavePosition.DeployPosition);
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
			move_.Execute(owner, owner.HavePosition.DeployPosition, true);
		}
	}
}
