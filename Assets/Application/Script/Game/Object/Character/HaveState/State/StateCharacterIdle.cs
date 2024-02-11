using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 待機状態
	/// </summary>
	public class StateCharacterIdle : IState<CharacterEntity> {

		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
            owner.HaveAnimation.Play(BattleMotion.Idle);
			owner.HaveUnitParam.Physical.CurrentSpeed = 0f;
		}

		/// <summary>
		/// 状態終了時処理
		/// </summary>
		public override void Exit(CharacterEntity owner) {
			//自然なばらつきが出るように遅延を入れる
			owner.HaveState.StateDelay.CalcDelay();
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CharacterEntity owner){
            //@todo 仮対応
            if (!owner.HaveAnimation.IsPlay(BattleMotion.Idle))
                owner.HaveAnimation.Play(BattleMotion.Idle);

			//クォータニオン計算はちょっと分解
			float sign = owner.Platoon.TowardSign;
			Quaternion quat = Quaternion.RotateTowards(owner.GetRotation(), new Quaternion(0f, (1f - sign) * 0.5f, 0f, (1f + sign) * 0.5f), owner.HaveUnitParam.Physical.AimSpeedDeg * Time.deltaTime);
			owner.SetRotation(quat);
		}
	}
}
