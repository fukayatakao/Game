using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 組織崩壊パラメータ
	/// </summary>
	[System.Serializable]
	public class DecayParam {
		[SerializeField]
		private float decay_;
		public float Decay { get { return decay_; } }

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Apply(int number, int maxNumber, bool isLeader) {
			//0で除算対策。元々一般ユニットがいない場合はLP漸減しない
			if (maxNumber == 0) {
				decay_ = 0f;
				return;
			}

			float num = number * (isLeader ? GameConst.Battle.LEADER_NUMBER_FOLD : 1);
			decay_ = 1f - ((num / maxNumber) / GameConst.Battle.DECAY_START_RANGE);
			decay_ = Mathf.Clamp01(decay_);
		}
	}
}
