using System;

namespace Project.Game {
	/// <summary>
	/// 状態チェック用
	/// </summary>
	[Serializable]
	public class ConditionSubBoard {
		//移動可能か
		private bool isMovable_;
		public bool IsMovable{ get{return isMovable_; } }
		//攻撃可能か
		private bool isAttackableState_;
		public bool IsAttackableState{ get{return isAttackableState_; } }
		//回復するか
		private bool isRecoverState_;
		public bool IsRecoverState { get { return isRecoverState_; } }

		//ノックバック無効
		public bool IsSuperArmor;
		//コリジョン無視フラグ
		public bool IsIgnoewCollision;
		//HP消費フラグ
		public bool IsLossHP;


		/// <summary>
		/// 定期的に自分の状態を監査
		/// </summary>
		public void Execute(CharacterEntity owner) {
			CharacterState.State state = owner.HaveState.CurrentState;

			isMovable_ = CheckMovable(state) && owner.HaveAction.IsCancelable();
			isAttackableState_ = CheckAttackable(state);
			isRecoverState_ = CheckRecover(state);
		}

		/// <summary>
		/// 移動可能か
		/// </summary>
		private bool CheckMovable(CharacterState.State state) {
			return state == CharacterState.State.Idle ||
					state == CharacterState.State.Attack ||
					state == CharacterState.State.Approach ||
					state == CharacterState.State.Move;
		}
		/// <summary>
		/// 攻撃可能か
		/// </summary>
		private bool CheckAttackable(CharacterState.State state) {
			return state == CharacterState.State.Idle ||
					state == CharacterState.State.Attack ||
					state == CharacterState.State.Approach ||
					state == CharacterState.State.Move;
		}
		/// <summary>
		/// 回復するか
		/// </summary>
		private bool CheckRecover(CharacterState.State state) {
			return state == CharacterState.State.Idle;
		}

	}
}
