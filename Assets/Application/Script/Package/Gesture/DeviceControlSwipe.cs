using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// スワイプ操作の検知
	/// </summary>
	public class DeviceControlSwipe {
		//スワイプ中判定
		private bool isSwipe_;
		public bool IsSwipe{ get { return isSwipe_; } }
		//処理有効フラグ
		private bool enable_;
		public bool Enable{ get { return enable_; } set { enable_ = value; isSwipe_ = false;} }
		//スワイプ中の変化距離
		private Vector2 deltaPos_;
		public Vector2 DeltaPos{ get { return deltaPos_; } }
		//スワイプ開始位置からの変化距離
		public Vector2 Direct{ get { return tempCurrentPos_ - tempBeginPos_; } }

		//フリック判定の開始時間、座標
		Vector2 tempBeginPos_;
		Vector2 tempCurrentPos_;

		/// <summary>
		/// 毎フレーム実行する処理
		/// </summary>
		public void Execute(DeviceTouch detectTouch){
			//検知行わないなら即終了
			if (!enable_)
				return;

			isSwipe_ = false;
			//タッチ開始時の時間と座標を取得
			if (detectTouch.IsTouchDown) {
				tempBeginPos_ = detectTouch.TouchPos;
				tempCurrentPos_ = tempBeginPos_;
			//スワイプ中判定
			} else if (detectTouch.IsTouch) {
				deltaPos_ = detectTouch.TouchPos - tempCurrentPos_;
                tempCurrentPos_ = detectTouch.TouchPos;

                if (deltaPos_.x != 0f || deltaPos_.y != 0f)
					isSwipe_ = true;
			}

		}
	}
}


