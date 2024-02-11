using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// キャラクターの行動指示関数
	/// </summary>
	[UnityEngine.Scripting.Preserve]
	public static class CharacterOrder {
		[Function("何もしない")]
        public static void Empty(CharacterEntity entity, object[] args) {
        }

        [Function("キャラクターの状態を{0}に設定")]
        [Arg(0, typeof(CharacterState.State), "ステート", CharacterState.State.None)]
        public static void ChangeState(CharacterEntity entity, object[] args) {
            CharacterState.State state = (CharacterState.State)args[0];
            entity.ChangeState(state);
        }

		[Function("キャラクターの列交代状態を{0}に設定")]
		[Arg(0, typeof(bool), "フラグ", false)]
		public static void SetSwapMode(CharacterEntity entity, object[] args) {
			bool flag = (bool)args[0];
			entity.HaveBlackboard.ConditionBoard.IsIgnoewCollision = flag;		//コリジョン無効
			entity.HaveBlackboard.ConditionBoard.IsSuperArmor = flag;			//ノックバック無効
			entity.HaveBlackboard.SwapBoard.IsSwap = flag;						//交代中フラグ
		}




		/*
		[Function("AI状態を{0}に設定")]
        [Arg(0, typeof(CharacterThink.State), "ステート", CharacterThink.State.None)]
		public static void ChangeAIState(CharacterEntity entity, object[] args) {
            CharacterThink.State state = (CharacterThink.State)args[0];
            entity.ChangeAIState(state);
        }

		[Function("LocalSwapフラグを{0}に設定")]
		[Arg(0, typeof(bool), "フラグ", false)]
		public static void SetLocalSwap(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.IsMobile = (bool)args[0];
		}

		[Function("コリジョン無効フラグを{0}に設定")]
		[Arg(0, typeof(bool), "フラグ", false)]
		public static void SetIgnoewCollision(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.ConditionBoard.IsIgnoewCollision = (bool)args[0];
		}

		[Function("ノックバック無効フラグを{0}に設定")]
		[Arg(0, typeof(bool), "フラグ", false)]
		public static void SetSuperArmor(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.ConditionBoard.IsSuperArmor = (bool)args[0];
		}

		[Function("交代中フラグを{0}に設定")]
		[Arg(0, typeof(bool), "フラグ", false)]
		public static void SetLineSwap(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.SwapBoard.IsSwap = (bool)args[0];
		}

		[Function("キャラクターの攻撃方法分岐")]
        [Arg(0, typeof(CharacterState.State), "通常攻撃", CharacterState.State.None)]
        [Arg(1, typeof(CharacterState.State), "強攻撃", CharacterState.State.None)]
        public static void ChangeAttackState(CharacterEntity entity, object[] args) {

			if (entity.HaveParam.Physical.CurrentSpeed < entity.HaveParam.Physical.MaxSpeed * BattleConst.CRUISE_RATE) {
				CharacterState.State state = (CharacterState.State)args[0];
				entity.ChangeState(state);
			} else {
				CharacterState.State state = (CharacterState.State)args[1];
				entity.ChangeState(state);
			}
        }


		[Function("目標座標を標的エネミーの位置で更新する")]
		public static void UpdateTaragetEnemyPos(CharacterEntity entity, object[] args) {
			Debug.Assert(entity.HaveBlackboard.EnemyBoard.TargetEnemy != null, "target enemy not found");
			entity.TargetTargetEnemy();
//			entity.HaveBlackboard.TargetPosition = entity.HaveBlackboard.EnemyBoard.TargetEnemy.GetPosition();
		}


		[Function("目標座標を再設定する")]
        public static void CalcTaragetPos(CharacterEntity entity, object[] args) {
			entity.TargetDeploy();

		}

		[Function("一番近い敵の位置を目標にする")]
		public static void ApproachNearEnemy(CharacterEntity entity, object[] args) {
			entity.TargetingNearEnemy();
		}
		[Function("標的の位置を目標にする")]
		public static void ApproachTargetEnemy(CharacterEntity entity, object[] args) {
			entity.TargetTargetEnemy();
		}
		[Function("AI変化の遅延を付ける")]
		public static void SetStateDelay(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.Delay = Time.time + DetermisticRandom.Range(0f, BattleConst.ORDER_DELAY_TIME);
		}
		[Function("AI変化の遅延をリセットする")]
		public static void ResetStateDelay(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.Delay = Time.time;
		}
		[Function("テスト")]
		[Arg(0, typeof(bool), "フラグ", false)]
		[Arg(1, typeof(bool), "フラグ", false)]
		[Arg(2, typeof(CharacterState.State), "通常攻撃", CharacterState.State.None)]
		[Arg(3, typeof(CharacterState.State), "強攻撃", CharacterState.State.None)]
		public static void test(CharacterEntity entity, object[] args) {
			entity.HaveBlackboard.Delay = Time.time;
		}*/
	}
}

