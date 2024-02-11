using UnityEditor;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// Editorのタイムラインの表示制御
	/// </summary>
	public class TimelineControlMain {
		EditorTimeController timeCtrl_ = new EditorTimeController();

        const float BUTTON_WIDTH = 50f;
        const float BUTTON_HEIGHT = 40f;

		const float DRAW_START_POS = BUTTON_WIDTH * 3;
		const float FRAME_RATE = 30f;
		public float CurrentTime {
                get {
                    timeCtrl_.CurrentTime = Mathf.Clamp(timeCtrl_.CurrentTime, 0f, MaxTime);
                    return timeCtrl_.CurrentTime;
                }
                set {
                    timeCtrl_.CurrentTime = value;
                }
            }

        public float MaxTime {
            get { return timeCtrl_.MaxTime ; }
            set { timeCtrl_.MaxTime = (value < 0f ? 0f : value); }
        }
        public bool IsPlaying{get{ return timeCtrl_.IsPlaying; } }


        System.Action play_;
        System.Action stop_;
        System.Action<float> seek_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelineControlMain(System.Action play, System.Action stop, System.Action<float> seek) {
            play_ = play;
            stop_ = stop;
            seek_ = seek;

            Init();
		}
		/// <summary>
		/// デフォルト値で初期化
		/// </summary>
		public void Init() {
			timeCtrl_.IsPlaying = false;
			timeCtrl_.IsRepeat = true;
			timeCtrl_.CurrentTime = 0f;
			timeCtrl_.MinTime = 0f;
			timeCtrl_.MaxTime = 0f;
        }
		/// <summary>
		/// GUI描画
		/// </summary>
		public Rect DrawGUI(EditorWindow window) {
            float width_ = window.position.width;

			//マイナス値は0に丸める
			if (timeCtrl_.MaxTime < 0f)
				timeCtrl_.MaxTime = 0f;
			//十字キー右で時間を進める
			if (Event.current.keyCode == KeyCode.RightArrow) {
				if (timeCtrl_.IsPlaying) timeCtrl_.Pause();
				timeCtrl_.CurrentTime += 0.01f;
			}

			//十字キー左で時間を戻す
			if (Event.current.keyCode == KeyCode.LeftArrow) {
				if (timeCtrl_.IsPlaying) timeCtrl_.Pause();
				timeCtrl_.CurrentTime -= 0.01f;
			}

			// Time
			using (new GUILayout.HorizontalScope()) {

				GUILayout.Label("Time", GUILayout.Width(DRAW_START_POS));

				Rect animTimeRect = GUILayoutUtility.GetLastRect();

				float num = 10f;
				float frameInterval = FRAME_RATE * timeCtrl_.MaxTime / num;
				float posInterval = (width_ - DRAW_START_POS) / num;

				for (int i = 0; i <= num; i++) {
					string time = string.Format("{0:D}:{1:D2}", (int)(frameInterval * i / FRAME_RATE), (int)(frameInterval * i % FRAME_RATE));
					GUI.Label(new Rect(DRAW_START_POS + posInterval * i - BUTTON_WIDTH, animTimeRect.y, animTimeRect.width, animTimeRect.height), (string.Format("{0:F2}", time)));
				}
			}

			GUILayout.Space(5);
			Rect animSliderRect;
			// Animation Slider
			using (new GUILayout.HorizontalScope()) {
				//アニメーションの再生・停止ボタン
				var btn = timeCtrl_.IsPlaying ? EditorGUIContent.PauseContent() : EditorGUIContent.PlayContent();
				if (GUILayout.Button(btn, EditorStyles.miniButton, GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT))) {
					if (timeCtrl_.IsPlaying) {
						timeCtrl_.Pause();
                        stop_();
					} else {
						timeCtrl_.Play();
                        play_();
                        seek_(timeCtrl_.CurrentTime);
                    }
                }

				//アニメーションのループon/off
				timeCtrl_.IsRepeat = GUILayout.Toggle(timeCtrl_.IsRepeat, EditorGUIContent.LoopContent(), EditorStyles.miniButton, GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT));
				EditorGUI.BeginChangeCheck();
				//現在の時間を設定
				timeCtrl_.CurrentTime = GUILayout.HorizontalSlider(timeCtrl_.CurrentTime, 0, timeCtrl_.MaxTime, "box", "Button", GUILayout.Height(BUTTON_HEIGHT), GUILayout.Width(width_ - DRAW_START_POS));
				if (EditorGUI.EndChangeCheck()) {
                    //me.SeekEvent(TimeCtrl.CurrentTime);
                    seek_(timeCtrl_.CurrentTime);
				}


				animSliderRect = GUILayoutUtility.GetLastRect();

			}
            EditorGUI.BeginChangeCheck();
            timeCtrl_.CurrentTime = EditorGUILayout.FloatField("Current Time:", timeCtrl_.CurrentTime);
            if (EditorGUI.EndChangeCheck()) {
                seek_(timeCtrl_.CurrentTime);
            }
            return animSliderRect;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Update() {
			float t = timeCtrl_.CurrentTime;
			timeCtrl_.TimeUpdate();
			if (t == timeCtrl_.CurrentTime)
				return;
            //リピート設定がないときは時間が来たら停止
            if (timeCtrl_.CurrentTime >= timeCtrl_.MaxTime && !timeCtrl_.IsRepeat) {
                timeCtrl_.Stop();
                stop_();
            }
            if (timeCtrl_.CurrentTime >= MaxTime && timeCtrl_.IsRepeat) {
                timeCtrl_.Reset();
                play_();
            }
        }

    }
}
