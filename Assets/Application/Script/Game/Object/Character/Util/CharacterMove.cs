using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 移動処理
	/// </summary>
	public class CharacterMove {
		Quaternion last_;
		bool rotateFlag_;
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public void Init(CharacterEntity owner, Vector3 targetPos) {
			last_ = MathUtil.LookAtY(targetPos - owner.GetPosition());
			rotateFlag_ = false;
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public void Execute(CharacterEntity owner, Vector3 targetPos, bool deaccelerate=false) {
			if (owner.HaveState.StateDelay.Delay > Time.time)
				return;

			//移動速度に応じてモーションを変化させる
			ChangeMotion(owner);


			Vector3 targetVec = targetPos - owner.GetPosition();
			float distSq = targetVec.sqrMagnitude;

			Quaternion targetQuat = MathUtil.LookAtY(targetVec);
			PhysicalParam physical = owner.HaveUnitParam.Physical;


			//ターゲットへの進行方向が前回移動の方向とは一定以上の角度がある場合、一旦減速して止まってから回転を始める
			if (Quaternion.Dot(targetQuat, last_) < GameConst.Battle.MOVABLE_ANGLE_COS) {
				if (rotateFlag_ == false) {

					//まだ減速してない場合は減速しきるまで旋回しないでここで終わる
					if (rotateFlag_ == false && owner.HaveUnitParam.Physical.CurrentSpeed > GameConst.Battle.STATIC_FRICTION) {
						physical.Deaccelerate();
						owner.Move(owner.GetRotation(), physical.CurrentSpeed * Time.deltaTime);
						return;
					}
					//減速が終わったら旋回開始して移動
					rotateFlag_ = true;
				}
			} else {
				rotateFlag_ = false;
			}
			//前回の進行方向を更新
			last_ = targetQuat;




			if (deaccelerate) {
				//目標の座標に近づいたら減速して止まる
				float slip = physical.CalcSlipDistance(physical.CurrentSpeed);
				if (slip * slip > distSq) {
					physical.Deaccelerate();
					if (physical.CurrentSpeed < GameConst.Battle.STATIC_FRICTION) {
						physical.CurrentSpeed = GameConst.Battle.STATIC_FRICTION;
					}
				} else {
					physical.Accelerate();
				}
			} else {
					physical.Accelerate();
			}
			owner.Move(targetQuat, physical.CurrentSpeed * Time.deltaTime);

			{
				Quaternion quat = Quaternion.RotateTowards(owner.GetRotation(), targetQuat, owner.HaveUnitParam.Physical.AimSpeedDeg * Time.deltaTime);
				owner.SetRotation(quat);
			}
		}

		/// <summary>
		/// 移動速度に応じてモーションを変化させる
		/// </summary>
		private void ChangeMotion(CharacterEntity owner) {
			//巡航速度以上なら走る
			if (owner.HaveUnitParam.Physical.CurrentSpeed > owner.HaveUnitParam.Physical.MaxSpeed * GameConst.Battle.CRUISE_RATE) {
				if (!owner.HaveAnimation.IsPlay(BattleMotion.Run))
					owner.HaveAnimation.Play(BattleMotion.Run);
				//即停止可能な速度以下なら待機モーションのまま
			} else if (owner.HaveUnitParam.Physical.CurrentSpeed < GameConst.Battle.STATIC_FRICTION) {
				if (!owner.HaveAnimation.IsPlay(BattleMotion.Idle))
					owner.HaveAnimation.Play(BattleMotion.Idle);
				//それ以外は歩く
			} else {
				if (!owner.HaveAnimation.IsPlay(BattleMotion.Walk))
					owner.HaveAnimation.Play(BattleMotion.Walk);
			}

		}

	}
}
