using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// Unity実行してない状態での時間制御
	/// </summary>
	public class EditorTimeController
	{
		//現在時間
		public float CurrentTime{ get; set; }

		public float MinTime { get; set; }
		public float MaxTime { get; set; }
		public bool IsPlaying { get; set; }
		public bool IsRepeat { get; set; }
		float lastTime_ = 0f;
		/// <summary>
		/// 時間更新
		/// </summary>
		public void TimeUpdate() {
            if (!IsPlaying)
                return;
			//Unity実行しているときはdeltaTimeを使う
			if (Application.isPlaying) {
				//EditorではTime.deltatimeが正しくないみたいなので実時間経過で処理する
				CurrentTime += (UnityEngine.Time.realtimeSinceStartup - lastTime_);
				lastTime_ = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		/// <summary>
		/// 再生
		/// </summary>
		public void Play() {
			IsPlaying = true;
			lastTime_ = UnityEngine.Time.realtimeSinceStartup;
		}

        /// <summary>
        /// 一時停止
        /// </summary>
        public void Pause() {
			IsPlaying = false;
		}


		/// <summary>
		/// 停止
		/// </summary>
		public void Stop() {
			IsPlaying = false;
			CurrentTime = 0;
		}

        /// <summary>
        /// 停止
        /// </summary>
        public void Reset() {
            CurrentTime = 0;
        }

    }
}
