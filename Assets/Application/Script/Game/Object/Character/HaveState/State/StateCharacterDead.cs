using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 死亡状態
	/// </summary>
	public class StateCharacterDead : IState<CharacterEntity> {

		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
            owner.HaveAnimation.Play(BattleMotion.Dead);
			owner.HaveUnitParam.Physical.CurrentSpeed = 0f;
		}

		/// <summary>
		/// 状態終了時処理
		/// </summary>
		public override void Exit(CharacterEntity owner) {
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CharacterEntity owner){
			//死亡モーションが終わったら
			if (owner.HaveAnimation.IsEnd()) {
				//ゲームから取り除く（非アクティブにして動かないようにする
				owner.Clear();
				//ステートは無効状態にしておく
				owner.ChangeState(CharacterState.State.None);
			}

		}
	}
}
