using System;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 攻撃選択関係の記憶
	/// </summary>
	[Serializable]
	public class AttackSubBoard {
		//すべての攻撃手段のうちの最大射程。これより遠い場合は攻撃可能チェックの必要がない。
		[SerializeField]
		float maxAttackDistance_;
		//選択された攻撃パターン
		[SerializeField]
		private Mst.MstActionData selectPattern_;
		public Mst.MstActionData SelectAttackPattern { get { return selectPattern_; } }

		/// <summary>
		/// 攻撃手段の有無をチェック
		/// </summary>
		public bool IsSelectAttack() {
			return SelectAttackPattern != null;
		}
		/// <summary>
		/// 最低限のカリング用に攻撃できる最大距離を拾っておく
		/// </summary>
		public void Init(Mst.MstUnitData master) {
			maxAttackDistance_ = float.MinValue;
			foreach (var attack in master.AttackPattern) {
				if (maxAttackDistance_ < attack.RangeMax)
					maxAttackDistance_ = attack.RangeMax;
			}
		}

		/// <summary>
		/// 定期的に近くの敵を検査
		/// </summary>
		public void Execute(CharacterEntity owner) {
			Calc(owner);
		}


		/// <summary>
		/// 攻撃手段の選択
		/// </summary>
		private void Calc(CharacterEntity entity) {
			//前回の選択をリセット
			selectPattern_ = null;
			CharacterEntity target = entity.HaveBlackboard.EnemyBoard.TargetEnemy;
			if (target == null) {
				return;
			}
			Vector3 vec = target.GetPosition() - entity.GetPosition();
			//キャラとキャラの距離を計算（キャラ半径を考慮する
			float distance = vec.magnitude - entity.HaveCollision.Radius - target.HaveCollision.Radius;
			if (distance > maxAttackDistance_)
				return;
			//キャラが重なっているとマイナスになる場合があるので0に丸める
			if (distance < 0f)
				distance = 0f;
			Vector3 direct = vec.normalized;

			Ray ray = new Ray(entity.GetPosition(), direct);
			//とりあえずは射手内の攻撃方法があればそれを選択
			foreach (Mst.MstActionData attack in entity.HaveUnitMaster.AttackPattern) {
				//射程の外
				if (distance < attack.RangeMin || distance > attack.RangeMax)
					continue;
				//発動に条件が必要なものは条件を満たしているかチェック
				if(!string.IsNullOrEmpty(attack.Check) && CharacterChecker.CheckMethodDict.ContainsKey(attack.Check)) {
					if (!CharacterChecker.CheckMethodDict[attack.Check](entity))
						continue;
				}


				//射線に味方がいないかチェックが必要。十分近い場合は判定を無視
				if (!attack.Indirect && distance < GameConst.Battle.IGNORE_FRIENDLY_ATTACK_DISTANCE) {
					bool isHit = Physics.Raycast(ray, out RaycastHit hit, attack.RangeMax, 1 << (int)UnityLayer.Layer.Character);
					if (!isHit)
						continue;
					CharacterPortal portal = hit.collider.gameObject.GetComponent<CharacterPortal>();
					//キャラ以外のものと当たるケースはないはず
					Debug.Assert(portal != null, "raycast check error");
					if (portal.Owner != target)
						continue;
				}

				selectPattern_ = attack;
				break;
			}

		}
	}
}

