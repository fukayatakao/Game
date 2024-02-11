using UnityEngine;
using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// ダメージを受けてよろけた状態
    /// </summary>
    public class StateCharacterDamage : IState<CharacterEntity> {
        Quaternion knockbackQuat;
		float remainTime_;
		float accelerate_;
        /// <summary>
        /// 状態開始時処理
        /// </summary>
        public override void Enter(CharacterEntity owner) {
            owner.HaveAnimation.Play(BattleMotion.Damage);
			owner.HaveBlackboard.KnockbackBorad.IsKnockback = true;
			knockbackQuat = MathUtil.LookAtY(owner.HaveBlackboard.KnockbackBorad.KnockBackVector);

			remainTime_ = Time.time + GameConst.Battle.KNOCKBACK_TIME;
			accelerate_ = owner.HaveBlackboard.KnockbackBorad.KnockBackFriction;
		}

        /// <summary>
        /// 状態終了時処理
        /// </summary>
        public override void Exit(CharacterEntity owner) {
			owner.HaveBlackboard.KnockbackBorad.IsKnockback = false;
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CharacterEntity owner) {
            if (owner.HaveBlackboard.KnockbackBorad.KnockBackSpeed < GameConst.Battle.STATIC_FRICTION && remainTime_ < Time.time) {
                owner.ChangeState(CharacterState.State.Idle);
                return;
            }

			owner.HaveBlackboard.KnockbackBorad.KnockBackSpeed -= accelerate_ * Time.deltaTime;
            //摩擦で減速した結果逆方向に移動しないようにマイナス値になったら0に丸める
            if(owner.HaveBlackboard.KnockbackBorad.KnockBackSpeed < 0f) {
				owner.HaveBlackboard.KnockbackBorad.KnockBackSpeed = 0f;
            }

            owner.Move(knockbackQuat, owner.HaveBlackboard.KnockbackBorad.KnockBackSpeed * Time.deltaTime);
        }
    }
}
