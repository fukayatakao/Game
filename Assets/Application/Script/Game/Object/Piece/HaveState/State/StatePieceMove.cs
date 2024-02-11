using UnityEngine;
using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 移動状態
    /// </summary>
    public class StatePieceMove : IState<PieceEntity> {
		float t;
		float dt;
		Geopolitics.Way way_;
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(PieceEntity owner) {
			way_ = owner.MoveWay;
			t = 0f;
			dt = 10f / way_.length;
		}

        /// <summary>
        /// 状態終了時処理
        /// </summary>
        public override void Exit(PieceEntity owner) {
			owner.HaveModel.HaveAnimation.Play(BattleMotion.Idle);
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(PieceEntity owner) {
			if (!owner.HaveModel.HaveAnimation.IsPlay(BattleMotion.Run))
				owner.HaveModel.HaveAnimation.Play(BattleMotion.Run);

			t += dt * Time.deltaTime;
			if (t > 1f)
				t = 1f;
			Vector3 pos = way_.Evaluate(t);

			Vector3 vec = pos - owner.GetPosition();
			owner.SetRotation(MathUtil.LookAtY(vec));

			owner.SetPosition(pos);
			owner.Grounding();

			if (t == 1f)
				owner.ChangeState(PieceState.State.None);
		}
	}
}
