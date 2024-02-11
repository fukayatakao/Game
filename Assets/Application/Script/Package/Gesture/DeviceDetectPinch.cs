using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// ピンチ入力の検知
	/// </summary>
	public class DeviceDetectPinch {
		public bool IsPinchIn{ get { return deltaDistance_ < 0f; } }
		public bool IsPinchOut{ get { return deltaDistance_ > 0f; } }
		//距離の変化分
		float deltaDistance_;
		public float DeltaDistance{
#if UNITY_EDITOR || UNITY_STANDALONE
            get { return deltaDistance_ * distanceRate_; }
#else
            get { return deltaDistance_; }
#endif
        }
        //処理有効フラグ
        private bool enable_;
		public bool Enable{ get { return enable_; } set { enable_ = value;} }

		float prevDistance_;
		float distance_;

        /// <summary>
        /// ピンチイン・アウトのチェック
        /// PCの場合はCommnadを押すとそこが2本目の指になる
        /// </summary>
        public void Execute() {
			//検知行わないなら即終了
			if (!enable_)
				return;

            deltaDistance_ = 0f;

            if (Input.touchCount >= 2) {
                // SmartPhone
                if (Input.touches[0].phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began) {
					PinchBegin (Input.touches [0].position, Input.touches [1].position);
				} else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) {
					Pinch (Input.touches [0].position, Input.touches [1].position);
				}
			}

            DetectMouse();

        }
#if UNITY_EDITOR || UNITY_STANDALONE
        private static float distanceRate_ = 10f;
        private static void SetMouseDistanceRate(float rate) {
            distanceRate_ = rate;
        }
#endif
        /// <summary>
        /// マウス操作の情報を取得
        /// </summary>
        [System.Diagnostics.Conditional( "UNITY_EDITOR" )]
        [System.Diagnostics.Conditional("UNITY_STANDALONE")]
        protected void DetectMouse(){
			if(Input.touchCount==0){
                deltaDistance_ = -Input.GetAxis("Mouse ScrollWheel");
			}
		}

		/// <summary>
		/// ピンチ開始
		/// </summary>
		private void PinchBegin(Vector2 pos1, Vector2 pos2){
			//距離を計算（ルート計算省きたいけど後々面倒になるだけなのでやめ。
			distance_ = (pos1 - pos2).magnitude;
		}

		/// <summary>
		/// ピンチ
		/// </summary>
		private void Pinch(Vector2 pos1, Vector2 pos2){
			//前回の距離を退避
			prevDistance_ = distance_;
			//距離を計算
			distance_ = (pos1 - pos2).magnitude;

			//距離の変化分を計算
			deltaDistance_ = distance_ - prevDistance_;
		}

	}
}


