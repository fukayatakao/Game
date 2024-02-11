using System;
using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// SP値管理
	/// </summary>
	public class PlatoonSpecialPoint : MonoPretender {
		[SerializeField]
		private int point_;
		public int Point { get { return point_; } }

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init() {
			point_ = 0;
		}

		/// <summary>
		/// リーダースキルを使用
		/// </summary>
		public bool UseSkill() {
			if (point_ < GameConst.Battle.SPECIAL_POINT_CONSUME)
				return false;

			point_ -= GameConst.Battle.SPECIAL_POINT_CONSUME;
			return true;
		}

		/// <summary>
		/// ダメージを受けた
		/// </summary>
		public void AddPoint(int point) {
			point_ += point;
			if (point_ > GameConst.Battle.SPECIAL_POINT_MAX)
				point_ = GameConst.Battle.SPECIAL_POINT_MAX;
		}

	}
}
