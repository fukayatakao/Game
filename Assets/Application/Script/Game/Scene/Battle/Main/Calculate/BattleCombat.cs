using Project.Http.Mst;
using UnityEngine;

namespace Project.Game {
    /// <summary>
    /// バトル計算の関数をまとめる
    /// </summary>
    public static class BattleCombat {
	    private static FieldEntity field_;
		private static BattlePhaseBonus bonus_;
		private static BattleTimer timer_;
		public static void Init(FieldEntity field, BattlePhaseBonus bonus, BattleTimer timer) {
		    field_ = field;
			bonus_ = bonus;
			timer_ = timer;
		}

		/// <summary>
		/// バトル計算
		/// </summary>
		public static void CalculateCombat(ActionDumpData dump) {
#if UNITY_EDITOR
			if (Project.Editor.ActEventEditorMain.IsActEventEditor())
				return;
#endif
			//entityがnullの場合はエラーにする。ゲーム上nullになるケースがある場合はこの関数の外で対処する
			Debug.Assert(dump != null, "dump is not set");
			Debug.Assert(dump.Actor != null, "actor is invalid");
            Debug.Assert(dump.Target != null, "target is invalid");
			Debug.Assert(dump.Attack != null, "not select attack:" + dump.Actor.gameObject.name);


			//死亡しているので計算不要
			if (!dump.Target.IsAlive)
				return;

			//ダメージ計算
			CalculateFight(dump.Actor, dump.Target, dump.Attack);

			//ダメージ計算の結果死亡してしまった
			if (!dump.Target.IsAlive) {
				dump.Target.Dead();
				return;
			}
			//ノックバック中は以降の計算はなし
			if (dump.Target.HaveBlackboard.KnockbackBorad.IsKnockback)
				return;
			//ノックバック計算
			bool result = CalcKnockBack(dump.Actor, dump.Target, dump.Attack);
			if (!result)
				return;
			//0 = v0 -at
			//v0=at
			//x = v0t - 1/2at^2
			//a=2x/t^2
			dump.Target.HaveBlackboard.KnockbackBorad.KnockBackFriction = (2 * dump.Target.HaveUnitMaster.Stagger * dump.Attack.Knockback) / (GameConst.Battle.KNOCKBACK_TIME * GameConst.Battle.KNOCKBACK_TIME);
			dump.Target.HaveBlackboard.KnockbackBorad.KnockBackSpeed = dump.Target.HaveBlackboard.KnockbackBorad.KnockBackFriction * GameConst.Battle.KNOCKBACK_TIME;
			dump.Target.HaveBlackboard.KnockbackBorad.KnockBackVector = dump.Target.GetPosition() - dump.Actor.GetPosition();
			if (!dump.Target.HaveBlackboard.ConditionBoard.IsSuperArmor) {
				dump.Target.HaveBlackboard.KnockbackBorad.Accumulate -= GameConst.Battle.KNOCKBACK_ACCUMULATE_LIMIT;
				dump.Target.ChangeState(CharacterState.State.Damage);
			}

		}



        /// <summary>
        /// 攻撃ダメージ計算
        /// </summary>
        private static void CalculateFight(CharacterEntity actor, CharacterEntity target, Mst.MstActionData attack) {
			float hpDamage = BattleCombat.CalcHPDamage(actor, target, attack);
			float lpDamage = BattleCombat.CalcLPDamage(actor, target, hpDamage);
			target.HaveUnitParam.Fight.Hp -= hpDamage;
			target.HaveUnitParam.Fight.Lp -= lpDamage;

			//ダメージを受けたことによるSP蓄積
			AccumulateSpecialPoint(target.Platoon);

#if RUN_BACKGROUND
			BattleLog.WriteDamage(actor, target, attack.Id, hpDamage, lpDamage, Time.time);
#endif
			BattleMessage.AttackReport.Broadcast(actor, target);
		}
		/// <summary>
		/// １人を対象にした攻撃
		/// </summary>
		private static float CalcHPDamage(CharacterEntity actor, CharacterEntity target, Mst.MstActionData attack) {
#if DEVELOP_BUILD
			//ダメージ計算無効にした場合
			if (!CalculateDebug.IsHpDamage) return 0;
#endif
			//プラス方向、マイナス方向それぞれでcosカーブ(-1～1)を描く。
			//それぞれの方向の差分が基準の半分以下では1ポイントあたりの効果が徐々に増加し、半分以上は効果が徐々に減少する
			//ダメージ式
			//(攻 - 防) >= 0
			//基準ダメージ + 基準ダメージ * (1 - cos(差 / 規定差分値 * π))
			//(攻 - 防) < 0
			//基準ダメージ - 基準ダメージ * (1 - cos(差 / 規定差分値 * π))
			float gradeRatio = (float)actor.HavePersonal.Grade / GameConst.Town.GRADE_SCALE;
			if (gradeRatio < 1f)
				gradeRatio = 1f;
			int atk = (int)(actor.HaveUnitParam.Fight.Attack * attack.Damage * gradeRatio);
			int def = (int)(target.HaveUnitParam.Fight.Defence);

			//標準ダメージ幅を超えた場合はカンスト
			int diff = Mathf.Clamp(atk - def, -GameConst.Battle.STANDARD_RANGE, GameConst.Battle.STANDARD_RANGE);
			int sign = diff >= 0 ? 1 : -1;
			int damage = (int)(GameConst.Battle.BASE_DAMAGE + sign * GameConst.Battle.BASE_DAMAGE * 0.5f * (1f - Mathf.Cos((Mathf.PI * diff / GameConst.Battle.STANDARD_RANGE))));
			if (damage < 0)
				damage = 0;

            return damage;
        }
		/// <summary>
		/// LPに対するダメージ値計算
		/// </summary>
		private static float CalcLPDamage(CharacterEntity actor, CharacterEntity target, float damage) {
#if DEVELOP_BUILD
			//ダメージ計算無効にした場合
			if (!CalculateDebug.IsLpDamage) return 0;
#endif
			//HPダメージ基準のLPダメージ
			//HPダメージ x 係数 x HP割合
			float hpRatio = Mathf.Clamp01(1f - target.HaveUnitParam.Fight.Hp / target.HaveUnitParam.Fight.MaxHp);
			float dmg = GameConst.Battle.LP_DAMAGE_RATIO * damage * hpRatio;


			//貫通>装甲時のダメージボーナス係数を計算
			float sub = GameConst.Battle.DIRECT_DAMAGE_RATIO * (actor.HaveUnitParam.Fight.Pircing - target.HaveUnitParam.Fight.Armor);
			if(sub < 0)
				sub = 0;
#if DEVELOP_BUILD
			if (!CalculateDebug.IsPircing) {
				sub = 0;
			}
#endif
			return dmg + sub;
        }


		/// <summary>
		/// ノックバック蓄積
		/// </summary>
		private static bool CalcKnockBack(CharacterEntity actor, CharacterEntity target, Mst.MstActionData attack) {
#if DEVELOP_BUILD
			//ノックバック強制にした場合
			if (CalculateDebug.IsForceKnockBackEnemy && target.Platoon.Index == Power.Enemy) return true;
			//ノックバック強制にした場合
			if (CalculateDebug.IsForceKnockBackPlayer && target.Platoon.Index == Power.Player) return true;
			//ノックバック無効にした場合
			if (!CalculateDebug.IsKnockBack) return false;
#endif
			PHASE phase = (PHASE)bonus_.current;
			if (actor.Squad.HaveParam.GetPhaseCount(phase) <= target.Squad.HaveParam.GetPhaseCount(phase))
				return false;

			target.HaveBlackboard.KnockbackBorad.Accumulate += attack.Impact;
			return target.HaveBlackboard.KnockbackBorad.Accumulate > GameConst.Battle.KNOCKBACK_ACCUMULATE_LIMIT;
        }

		/// <summary>
		/// ユニットのHP回復計算
		/// </summary>
		/// <remarks>
		/// 小隊メンバーの最前列にいるキャラとの距離で回復量が決定される(0~1倍の範囲)
		/// </remarks>
		public static void CalculateRecover(CharacterEntity owner) {
			//ゲーム時間が止まっているときは処理しない
			if (timer_ == null || !timer_.IsEnable())
				return;
			float Recovery = owner.HaveUnitParam.Fight.Recovery;
			//最前列と自分との距離を測る
			float distance = (owner.Platoon.HaveLimit.ForwardLimit - owner.GetPosition().z) * owner.Platoon.TowardSign;
			//回復量の倍率を計算
			float ratio = Mathf.Clamp(distance, 0f, GameConst.Battle.FULL_RECOVERY_DISTANCE) / GameConst.Battle.FULL_RECOVERY_DISTANCE;
			owner.HaveUnitParam.Fight.Hp += Recovery * Time.deltaTime * ratio;
		}

		/// <summary>
		/// ユニットのLP漸減計算
		/// </summary>
		/// <remarks>
		/// 自軍ユニットが減った状態では時間経過と共にLPが減少して逃亡する
		/// </remarks>
		public static void CalculateDecay(CharacterEntity owner) {
			//ゲーム時間が止まっているときは処理しない
			if (timer_ == null || !timer_.IsEnable())
				return;
			owner.HaveUnitParam.Fight.Lp -= owner.Squad.HaveParam.Decay * Time.deltaTime;
		}

		/// <summary>
		/// 部隊熟練度ボーナスを加算
		/// </summary>
		/// <remarks>
		/// 熟練度に応じて回復量の基本値にボーナス(1~2倍の範囲)
		/// </remarks>
		public static void AddExperienceBonus(CharacterEntity owner, float experience) {
			float bonus = experience / GameConst.Battle.EXPERIENCE_SCALE;
			owner.HaveUnitParam.Fight.Recovery += owner.HaveUnitParam.Fight.Recovery * bonus;
		}

		/// <summary>
		/// SP値の増加を計算
		/// </summary>
		/// <remarks>
		/// 攻撃を受けるたびに1プラス
		/// </remarks>
		private static void AccumulateSpecialPoint(PlatoonEntity platoon) {
			platoon.HaveSpecialPoint.AddPoint(1);
		}
	}
}
