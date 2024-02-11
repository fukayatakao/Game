using UnityEngine;

namespace Project.Lib {

	public class DeviceTouch{
		//現在のフレームのタッチ状態を取得
		protected TouchPhase phase_= TouchPhase.Canceled;
		public bool IsTouchDown{ get { return phase_ == TouchPhase.Began; } }
		public bool IsTouchUp{ get { return phase_ == TouchPhase.Ended; } }
		public bool IsTouch{ get { return phase_ != TouchPhase.Ended && phase_ != TouchPhase.Canceled; } }

		protected Vector2 touchPos_ = new Vector2();
		public Vector2 TouchPos{ get { return touchPos_; } }

		protected int fingerId_;

		/// <summary>
		/// マウス操作の情報を取得
		/// </summary>
		[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        protected void DetectMouse(int index = 0){
			if(Input.touchCount==0){
				//マウス操作からタッチイベントの関数を呼ぶ(UnityEditor用
				if(Input.GetMouseButtonDown(index)){
					touchPos_ = Input.mousePosition;
					phase_ = TouchPhase.Began;
				} else if(Input.GetMouseButtonUp(index)){
					touchPos_ = Input.mousePosition;
					phase_ = TouchPhase.Ended;
				} else if(Input.GetMouseButton(index)){
					touchPos_ = Input.mousePosition;
					phase_ = TouchPhase.Moved;
				}else{
					phase_ = TouchPhase.Canceled;
				}
			}
		}
#if DEVELOP_BUILD
		public void TouchCancel() {
			touchPos_.x = -float.MaxValue;
			touchPos_.y = -float.MaxValue;
			phase_ = TouchPhase.Canceled;
		}
#endif
	}


	/// <summary>
	/// タッチの入力を抽象化してイベントを処理
	/// </summary>
	/// <remarks>
	/// タッチスクリーンとマウスのデバイス入力の違いを吸収して共通処理化する
	/// </remarks>
	public class DeviceDetectTouch : DeviceTouch{

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(){
			if(Input.touchCount>0){
				if (!IsTouch) {
					DetectTouchStart ();
				} else {
					DetectTouchContinue ();
				}
			}

#if UNITY_EDITOR || UNITY_STANDALONE
            DetectMouse();
#endif
		}
		/// <summary>
		/// 使用するタッチの情報を取得
		/// </summary>
		private void DetectTouchStart(){
			for (int i = 0; i < Input.touchCount; i++) {
				Touch touch = Input.touches [i];
				//新たに開始したfingerIdをタップ情報として使う
				if (touch.phase == TouchPhase.Began) {
					phase_ = touch.phase;
					touchPos_ = touch.position;
					fingerId_ = touch.fingerId;
					return;
				}
			}
		}



		/// <summary>
		/// 使用するタッチの情報を取得
		/// </summary>
		private void DetectTouchContinue(){
			//@note touchesの配列位置と指Idの関係が前のフレームと同一である保証はないのでタッチした指Idと一致する情報を探して処理を行う。
			for (int i = 0; i < Input.touchCount; i++) {
				Touch touch = Input.touches [i];
				if (fingerId_ == touch.fingerId) {
					phase_ = touch.phase;
					touchPos_ = touch.position;
					return;
				}
			}

			//検知したタッチの指idが終了を検知しないでいきなり消えていたら(そんなケースあるのか？)Assert出す
			Debug.Assert(false, "illegal touch detect");

			//一応終了扱いにする
			phase_ = TouchPhase.Canceled;
		}

	}
}


