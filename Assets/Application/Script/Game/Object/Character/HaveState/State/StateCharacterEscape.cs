using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 逃亡状態
	/// </summary>
	public class StateCharacterEscape : IState<CharacterEntity> {
		CharacterMove move_ = new CharacterMove();

		private float time_;
		private Vector3 target_;
		private const float EscapeSpeed = 10f;
		private const float EscapeTime = 2f;
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
			//Delayが入っているのでリセットして即動かす
			owner.HaveState.StateDelay.Reset();
			target_ = owner.GetPosition();
			target_.z = -owner.Platoon.TowardSign * GameConst.Battle.LONG_DISTANCE;

			move_.Init(owner, target_);
			//逃亡用の最大速度に変える
			owner.HaveUnitParam.Physical.MaxSpeed = 10f;
			owner.HaveUnitParam.Physical.CurrentSpeed = 10f;

			owner.HaveUnitParam.Physical.AimSpeedDeg = 720f;
			time_ = 0f;
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
			move_.Execute(owner, target_);
			time_ += Time.deltaTime;

			//死亡モーションが終わったら
			if (time_ > EscapeTime) {
				owner.HaveUnitParam.Physical.Init(owner.HaveUnitMaster);
				//ゲームから取り除く（非アクティブにして動かないようにする
				owner.Clear();
				//ステートは無効状態にしておく
				owner.ChangeState(CharacterState.State.None);
			}

		}
	}
}
