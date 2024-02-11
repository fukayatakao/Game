using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// Timeの操作
	/// </summary>
	public static class TimeControl {
		//ロックカウンター
		private static int counter_;
		private static float scale_;
		/// <summary>
		/// 初期化
		/// </summary>
		public static void Initialize() {
			counter_ = 0;
			scale_ = 1f;
		}
		/// <summary>
		/// 時間経過を止める
		/// </summary>
		public static void Lock() {
			counter_++;
			Time.timeScale = 0f;
		}

		/// <summary>
		/// 時間経過を止めを解除する
		/// </summary>
		public static void UnLock() {
			counter_--;
			if (counter_ == 0) {
				Time.timeScale = scale_;
			}

			Debug.Assert(counter_ >= 0, "counter error:" + counter_);
		}

		public static void ResetScale() {
			scale_ = 1f;
		}
		public static void SlowScale() {
			scale_ = scale_ * 0.75f;
			if (scale_ < 0.1f) {
				scale_ = 0.1f;
			}
			Time.timeScale = scale_;
		}
		public static void FastScale() {
			scale_ = scale_ / 0.75f;
			if (scale_ > 4f) {
				scale_ = 4f;
			}
			Time.timeScale = scale_;
		}
	}

}
