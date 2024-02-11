using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// キャラクターの行動評価関数
	/// </summary>
	[UnityEngine.Scripting.Preserve]
	public static class CharacterEvaluate {
        [Function("常に成功")]
        public static bool Always(CharacterEntity entity, object[] args) {
            return true;
        }
        [Function("常に失敗")]
        public static bool Never(CharacterEntity entity, object[] args) {
	        return false;
        }

		[Function("{0}番目の列か")]
		[Arg(0, typeof(Abreast), "列順", Abreast.First)]
		public static bool CheckAbreast(CharacterEntity entity, object[] args) {
			return (Abreast)args[0] == entity.Squad.Orbat;
		}
		[Function("{0}番目の列以外か")]
		[Arg(0, typeof(Abreast), "列順", Abreast.First)]
		public static bool CheckNotAbreast(CharacterEntity entity, object[] args) {
			return (Abreast)args[0] != entity.Squad.Orbat;
		}

		[Function("展開位置と現在位置の差が{0}m以上の場合")]
		[Arg(0, typeof(float), "距離", 0f)]
		public static bool CheckFarDeployTarget(CharacterEntity entity, object[] args) {
			float distanceSq = (entity.GetPosition() - entity.HavePosition.DeployPosition).sqrMagnitude;
			float d = (float)args[0];
			return distanceSq >= d * d;
		}

		[Function("展開位置と現在位置の差が{0}m以上の前方の場合")]
		[Arg(0, typeof(float), "距離", 0f)]
		public static bool CheckForwardDeployTarget(CharacterEntity entity, object[] args) {
			float distanceSq = (entity.GetPosition() - entity.HavePosition.DeployPosition).sqrMagnitude;
			float d = (float)args[0];

			bool result = BattleUtil.IsForward(entity.GetPosition().z, entity.HavePosition.DeployPosition.z, entity.Platoon.TowardSign, 0);

			return result && distanceSq >= d * d;
		}

		[Function("展開位置と現在位置の差が{0}m以上の後方の場合")]
		[Arg(0, typeof(float), "距離", 0f)]
		public static bool CheckBackwardDeployTarget(CharacterEntity entity, object[] args) {
			float distanceSq = (entity.GetPosition() - entity.HavePosition.DeployPosition).sqrMagnitude;
			float d = (float)args[0];

			bool result = BattleUtil.IsBackward(entity.GetPosition().z, entity.HavePosition.DeployPosition.z, entity.Platoon.TowardSign, 0);

			return result && distanceSq >= d * d;
		}

		[Function("展開位置と現在位置の差が{0}m未満の場合")]
		[Arg(0, typeof(float), "距離", 0f)]
		public static bool CheckNearDeployTarget(CharacterEntity entity, object[] args) {
			float distanceSq = (entity.GetPosition() - entity.HavePosition.DeployPosition).sqrMagnitude;
			float d = (float)args[0];
			return distanceSq < d * d;
		}

		[Function("キャラクターのステートが{0}の場合")]
		[Arg(0, typeof(CharacterState.State), "ステート", CharacterState.State.None)]
		public static bool CheckCharacterState(CharacterEntity entity, object[] args) {
			return entity.HaveState.IsCurrentState((CharacterState.State)args[0]);
		}

		[CheckFunction("展開座標に到達している", "展開座標に到達していない")]
		public static bool IsInDeployPos(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HavePosition.CheckDeployPos(entity.GetPosition());
			} else {
				return !entity.HavePosition.CheckDeployPos(entity.GetPosition());
			}
		}

		[CheckFunction("攻撃範囲内に敵がいる", "攻撃範囲内に敵がいない")]
		public static bool IsInAttackRange(CharacterEntity entity, object[] args) {
			return (bool)args[0] == entity.HaveBlackboard.AttackBoard.IsSelectAttack();
		}


		[CheckFunction("索敵範囲内に敵がいる", "索敵範囲内に敵がいない")]
		public static bool IsInSearchRange(CharacterEntity entity, object[] args) {
			bool result = entity.IsInSearchRangeEnemy();

			if ((bool)args[0]) {
				return result;
			} else {
				return !result;
			}
		}

		[Function("自分が所属列の範囲+{0}m以内にいる")]
		[Arg(0, typeof(float), "距離", 0f)]
		public static bool CheckInnerLineRange(CharacterEntity entity, object[] args) {
			SquadEntity group = entity.Squad;
			float offset_z = (float)args[0];
			if (!group.IsBackward(entity.GetPosition().z, offset_z) && !group.IsForward(entity.GetPosition().z, offset_z)) {
				return true;
			}
			return false;
		}
		[CheckFunction("移動可能な状態", "移動不可能な状態")]
		public static bool IsMovable(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HaveBlackboard.ConditionBoard.IsMovable;
			} else {
				return !entity.HaveBlackboard.ConditionBoard.IsMovable;
			}
		}

		[CheckFunction("攻撃可能な状態", "攻撃不可能な状態")]
		public static bool IsAttackable(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HaveBlackboard.ConditionBoard.IsAttackableState;
			} else {
				return !entity.HaveBlackboard.ConditionBoard.IsAttackableState;
			}
		}

		[CheckFunction("Swap列よりも前にいる", "Swap列よりも後ろにいる")]
		public static bool IsSwapOver(CharacterEntity entity, object[] args) {
			SquadEntity target = entity.HaveBlackboard.SwapBoard.SwapTarget;
			if (target == null)
				return true;

			if ((bool)args[0]) {
				if (target.IsForward(entity.GetPosition().z)) {
					return true;
				} else {
					return false;
				}
			} else {
				if (target.IsBackward(entity.GetPosition().z)) {
					return true;
				} else {
					return false;
				}
			}
		}
		[CheckFunction("敵が後方にしかいない", "敵が前方に残っている")]
		public static bool GoThrough(CharacterEntity entity, object[] args) {
			PlatoonEntity enemyPlatoon = entity.Platoon.HaveBlackboard.Opponent;
			if ((bool)args[0]) {
				return enemyPlatoon.IsBackward(entity.GetPosition().z);
			} else {
				return !enemyPlatoon.IsBackward(entity.GetPosition().z);
			}
		}









		/*

		[CheckFunction("ターゲットしている敵がいる", "ターゲットしている敵がいない")]
		public static bool CheckTargetEnemy(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HaveBlackboard.EnemyBoard.TargetEnemy != null;
			} else {
				return entity.HaveBlackboard.EnemyBoard.TargetEnemy == null;
			}
		}

		[CheckFunction("目標座標に到達している", "目標座標に到達していない")]
		public static bool IsInTargetPos(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HavePosition.CheckDeployPos(entity.GetPosition());
			} else {
				return !entity.HavePosition.CheckDeployPos(entity.GetPosition());
			}
		}

		[CheckFunction("一番近い敵がボス", "一番近い敵がボスではない")]
		public static bool IsNearEnemyBoss(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HaveBlackboard.EnemyBoard.IsNearBoss;
			} else {
				return !entity.HaveBlackboard.EnemyBoard.IsNearBoss;
			}
		}
		[CheckFunction("命令の遅延中", "命令の遅延が過ぎた")]
		public static bool SetStateDelay(CharacterEntity entity, object[] args) {
			if ((bool)args[0]) {
				return entity.HaveBlackboard.Delay > Time.time;
			} else {
				return entity.HaveBlackboard.Delay <= Time.time;
			}
		}*/
	}
}
