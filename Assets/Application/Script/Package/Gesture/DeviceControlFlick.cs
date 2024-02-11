using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// フリック操作の検知
	/// </summary>
	public class DeviceControlFlick {
		//フリック判定
		private bool isFlick_;
		public bool IsFlick{ get { return isFlick_; } }
		//処理有効フラグ
		private bool enable_;
		public bool Enable{ get { return enable_; } set { enable_ = value; isFlick_ = false;} }
		//フリック判定したときのタッチ距離
		private Vector2 direct_;
		public Vector2 Direct{ get { return direct_; } }

		//フリックと判断する時間と距離
		private const float DEFAULT_FLICK_TIME		= 0.35f;
		private const float DEFAULT_FLICK_DISTANCE	= 32f;	// フリックと判定するデフォルト距離（5段階なので注意10,32~）

		private float time_ = DEFAULT_FLICK_TIME;
		public float AccurateTime{ get { return time_; } set { time_ = value; } }

		private float distanceSq_ = DEFAULT_FLICK_DISTANCE * DEFAULT_FLICK_DISTANCE;
		public float AccurateDistanceSq{ get { return distanceSq_; } set { distanceSq_ = value; } }

		//フリック判定の開始時間、座標
		float tempBeginTime_;
		Vector2 tempBeginPos_;

		/// <summary>
		/// フリック判定の時間を設定
		/// </summary>
		public void SetFlickTime(float t, float distance){
			time_ = t;
		}
		/// <summary>
		/// フリック判定の距離を設定
		/// </summary>
		public void SetFlickDistance(float distance){
			distanceSq_ = distance * distance;
		}

		/// <summary>
		/// 毎フレーム実行する処理
		/// </summary>
		public void Execute(DeviceTouch detectTouch){
			//検知行わないなら即終了
			if (!enable_)
				return;

			isFlick_ = false;
			//タッチ開始時の時間と座標を取得
			if (detectTouch.IsTouchDown) {
				tempBeginTime_ = Time.time;
				tempBeginPos_ = detectTouch.TouchPos;

			//タッチ終了時の時間と座標からフリック判定
			} else if (detectTouch.IsTouchUp) {
				float touchTime = Time.time - tempBeginTime_;
				direct_ = detectTouch.TouchPos - tempBeginPos_;

				if (touchTime < time_ && direct_.sqrMagnitude > distanceSq_) {
					isFlick_ = true;
				}
			}

		}
	}
}


