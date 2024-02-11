using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
	/// <summary>
	/// タッチの入力を抽象化してイベントを処理
	/// </summary>
	/// <remarks>
	/// タッチスクリーンとマウスのデバイス入力の違いを吸収して共通処理化する
	/// </remarks>
	public class DeviceDetectMultiTouch : DeviceTouch{
		//そのフレームでタッチを開始したfingerId
		private static List<int> startFingerList_ = new List<int> ();

		private static bool IsExistFinger(int id){
			for (int i = 0, max = startFingerList_.Count; i < max; i++) {
				if (startFingerList_ [i] == id) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(int index = 0){
			//@note startFingerList_はstaticなのでフレームごとに１回だけ処理
			//タッチ開始したfingerIdのリストをクリア
			if (index == 0)
				startFingerList_.Clear ();

			if(Input.touchCount>0){
				if (!IsTouch) {
					DetectTouchStart ();
				} else {
					DetectTouchContinue ();
				}
			}

#if UNITY_EDITOR || UNITY_STANDALONE
            DetectMouse(index);
#endif


		}
		/// <summary>
		/// 開始したタッチ(fingerId不明)の情報を取得
		/// </summary>
		private void DetectTouchStart(){
			for (int i = 0; i < Input.touchCount; i++) {
				Touch touch = Input.touches [i];
				//新たに開始したfingerIdをタップ情報として使う
				if (touch.phase == TouchPhase.Began && !IsExistFinger(touch.fingerId)) {
					phase_ = touch.phase;
					touchPos_ = touch.position;
					fingerId_ = touch.fingerId;
					startFingerList_.Add (fingerId_);
					return;
				}
			}
		}



		/// <summary>
		/// 継続している(fingerIdが判明している)タッチの情報を取得
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

