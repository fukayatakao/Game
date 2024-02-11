using UnityEngine;
using System;

namespace Project.Game {
	/// <summary>
	/// ノックバック関係の記憶
	/// </summary>
	[Serializable]
	public class KnockbackSubBoard {
		public KnockbackSubBoard() {
			Accumulate = DeterminateRandom.Range(0, GameConst.Battle.KNOCKBACK_ACCUMULATE_LIMIT);
		}


		public bool IsKnockback;

		public float KnockBackFriction;

		//ノックバックの初速
		public float KnockBackSpeed;
		//ノックバック方向(not単位ベクトル)
		public Vector3 KnockBackVector;
		//蓄積値
		public float Accumulate;

	}
}

