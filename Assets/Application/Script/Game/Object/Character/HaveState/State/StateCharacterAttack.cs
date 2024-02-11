using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 攻撃状態
	/// </summary>
	public class StateCharacterAttack : IState<CharacterEntity> {
		//アクションクールタイム
		private float actionInterval_;
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
			owner.HaveUnitParam.Physical.CurrentSpeed = 0f;
			actionInterval_ = 0f;
		}

		/// <summary>
		/// 状態終了時処理
		/// </summary>
		public override void Exit(CharacterEntity owner) {
			owner.HaveAction.Stop();
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CharacterEntity owner){
			actionInterval_ -= Time.deltaTime;
			if (actionInterval_ < 0f)
				actionInterval_ = 0f;



			CharacterEntity target = owner.HaveBlackboard.EnemyBoard.TargetEnemy;
			if (target == null)
				return;
			//攻撃する手段がない場合はここで終わり
			if (!owner.HaveBlackboard.AttackBoard.IsSelectAttack())
				return;
			Mst.MstActionData attackPattern = owner.HaveBlackboard.AttackBoard.SelectAttackPattern;

			//ターゲットの方向を向く
			//回転量に応じた向き直りを行う
			Quaternion quat = CharacterUtil.CalcAimTarget(owner.GetPosition(), owner.GetRotation(), target.GetPosition(), owner.HaveUnitParam.Physical.AimSpeedDeg);
			owner.SetRotation(quat);

			Vector3 vec = (target.GetPosition() - owner.GetPosition()).normalized;


			//正面方向にいるか
			float rangeCos = Mathf.Cos(attackPattern.AngleRad * 0.5f);
			float dot = Vector3.Dot(owner.CacheTrans.forward, vec);
			if (dot < rangeCos)
				return;

			//敵が攻撃範囲に居て、行動可能になったら
			if (actionInterval_ == 0f) {
				if (!owner.HaveAction.IsPlay()) {
					owner.HaveBlackboard.ActionDump = new ActionDumpData(owner, target, attackPattern);
					owner.HaveAction.Play(attackPattern.ActionName);
					actionInterval_ = attackPattern.Interval;
                }
            } else {
				if (owner.HaveAnimation.IsEnd()) {
					owner.HaveAnimation.Play(BattleMotion.Idle);
				}
			}

		}
	}
}
