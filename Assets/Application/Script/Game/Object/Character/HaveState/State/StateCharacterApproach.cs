using UnityEngine;
using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 移動状態
    /// </summary>
    public class StateCharacterApproach : IState<CharacterEntity> {
		CharacterMove move_ = new CharacterMove();
		/// <summary>
		/// 状態開始時処理
		/// </summary>
		public override void Enter(CharacterEntity owner) {
			//基本的にないはずだが、タイミングによってはあり得そうなのでチェック
			Vector3 pos = owner.HaveBlackboard.EnemyBoard.TargetEnemy == null ? owner.GetPosition() : owner.HaveBlackboard.EnemyBoard.TargetEnemy.GetPosition();
			move_.Init(owner, pos);
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
			//敵が死ぬなどして一時的に居なくなった場合
			//@note 不自然な動きになるようならAIが切り替わるまでは前回の座標に対して移動させるようにするべきか？
			if (owner.HaveBlackboard.EnemyBoard.TargetEnemy == null)
				return;
			move_.Execute(owner, owner.HaveBlackboard.EnemyBoard.TargetEnemy.GetPosition(), false);
		}

	}
}
