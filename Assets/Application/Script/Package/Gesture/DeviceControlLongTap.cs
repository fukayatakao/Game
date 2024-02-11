using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// 長押し操作の検知
	/// </summary>
	public class DeviceControlLongTap {
		//長押し判定
		private bool isLongTap_;
		public bool IsLongTap{ get { return isLongTap_; } }
		//処理有効フラグ
		private bool enable_;
		public bool Enable{ get { return enable_; } set { enable_ = value; isLongTap_ = false;} }
		//長押し判定の時間
		private float longTapTime_;
		public float LongTapTime{ get { return longTapTime_; } }

		//長押しと判断する時間と距離
		private const float DEFAULT_FLICK_TIME		= 0.35f;
		private float time_ = DEFAULT_FLICK_TIME;
		public float AccurateTime{ get { return time_; } set { time_ = value; } }


		Vector2 beginPos_;
		//長押し判定の開始時間
		float tempBeginTime_;

		/// <summary>
		/// 毎フレーム実行する処理
		/// </summary>
		public void Execute(DeviceTouch detectTouch){
			//検知行わないなら即終了
			if (!enable_)
				return;

			isLongTap_ = false;
			//タッチ開始時の時間と座標を取得
			if (detectTouch.IsTouchDown) {
				tempBeginTime_ = Time.time;
				beginPos_ = detectTouch.TouchPos;
			//タッチ終了時の時間と座標からフリック判定
			} else if (detectTouch.IsTouch) {
				if (beginPos_ != detectTouch.TouchPos) {
					tempBeginTime_ = Time.time;
				}
				longTapTime_ = Time.time - tempBeginTime_ - time_;

				if (longTapTime_ > 0f) {
					isLongTap_ = true;
				}
			}

		}
	}
}


