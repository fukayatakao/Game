using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 分隊の前限と後限位置の検出用
	/// </summary>
	public class SquadLimit : MonoPretender {
		//分隊に属するキャラの現在位置(z)の前限と後限の値
		[SerializeField]
		private float forwardLimit_;
		public float ForwardLimit { get { return forwardLimit_; } }

		[SerializeField]
		private float backwardLimit_;
		public float BackwardLimit { get { return backwardLimit_; } }

		/// <summary>
		/// 定期的に状態を記録
		/// </summary>
		public void Execute(SquadEntity owner) {
			float min = float.MaxValue;
			float max = float.MinValue;
			//所属キャラの情報を精査
			if(owner.Members != null) {
				for (int i = 0, count = owner.Members.Count; i < count; i++) {
					CalcMinMax(owner.Members[i], ref min, ref max);
				}
			}
			if(owner.Leader != null) {
				CalcMinMax(owner.Leader, ref min, ref max);
			}

			//結果を記録
			backwardLimit_ = owner.Platoon.TowardSign < 0 ? max : min;
			forwardLimit_ = owner.Platoon.TowardSign < 0 ? min : max;
		}

		/// <summary>
		/// limitのmix,maxを計算
		/// </summary>
		private void CalcMinMax(CharacterEntity entity, ref float min, ref float max) {
			Debug.Assert(entity != null, "character is null");
			//一番前と後ろのz座標を記録する
			CharacterEntity chara = entity;
			//個別入れ替え中の場合は前後位置の判定に加えない
			if (chara.HaveBlackboard.IsMobile) {
				return;
			}
			float r = chara.HaveCollision.Radius;
			float z = chara.GetPosition().z;
			if (min > z - r) {
				min = z - r;
			}
			if (max < z + r) {
				max = z + r;
			}
		}
	}
}

