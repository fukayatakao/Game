using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 小隊の前後限方法
    /// </summary>
    public class PlatoonLimit : CommonBlackboard {
		//分隊に属するキャラの現在位置(z)の前限と後限の値
		private float forwardLimit_;
		public float ForwardLimit { get { return forwardLimit_; } }

		private float backwardLimit_;
		public float BackwardLimit { get { return backwardLimit_; } }

		/// <summary>
		/// 定期的に状態を記録
		/// </summary>
		public void Execute(PlatoonEntity owner) {
			float backward;
			float forward;

			if (owner.TowardSign > 0f) {
				backward = float.MaxValue;
				forward = float.MinValue;
			} else {
				backward = float.MinValue;
				forward = float.MaxValue;
			}

			//所属キャラの情報を精査
			for (int i = 0, count = owner.Squads.Count; i < count; i++) {
				//一番前と後ろのz座標を記録する
				SquadEntity squad = owner.Squads[i];

				if(owner.TowardSign > 0f) {
					if (backward > squad.HaveLimit.BackwardLimit) {
						backward = squad.HaveLimit.BackwardLimit;
					}
					if (forward < squad.HaveLimit.ForwardLimit) {
						forward = squad.HaveLimit.ForwardLimit;
					}
				} else {
					if (backward < squad.HaveLimit.BackwardLimit) {
						backward = squad.HaveLimit.BackwardLimit;
					}
					if (forward > squad.HaveLimit.ForwardLimit) {
						forward = squad.HaveLimit.ForwardLimit;
					}
				}
			}

			//結果を記録
			backwardLimit_ = backward;
			forwardLimit_ = forward;
		}


	}
}

