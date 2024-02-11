using UnityEditor;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// Editorのタイムラインの表示制御
	/// </summary>
	public class TimelineControlSub{
        const float BUTTON_HEIGHT = 20f;

		const float DRAW_START_POS = 240;
		const float FRAME_RATE = 30f;

        private float currentTime_;
        public float CurrentTime {
            get {
                currentTime_ = Mathf.Clamp(currentTime_, 0f, MaxTime);
                return currentTime_;
            }
            set {
                currentTime_ = value;
            }
        }
		//HeadTimeは描画時に毎回セットし直してる
		public float HeadTime;
        public float MaxTime;

        public AnimationClip Clip = null;


        public static System.Action<float> SeekEvent;

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TimelineControlSub() {
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public TimelineControlSub(TimelineControlSub src) {
			HeadTime = src.HeadTime;
			MaxTime = src.MaxTime;

			Clip = src.Clip;
		}

		/// <summary>
		/// GUI描画
		/// </summary>
		public Rect DrawSlider(EditorWindow window) {
            float width = window.position.width;

			Rect animSliderRect;
			// Animation Slider
			EditorGUI.BeginChangeCheck();
			//現在の時間を設定
			CurrentTime = GUILayout.HorizontalSlider(CurrentTime, 0, MaxTime, "box", "Button", GUILayout.Height(BUTTON_HEIGHT), GUILayout.Width(width - DRAW_START_POS));
			if (EditorGUI.EndChangeCheck()) {
                SeekEvent(HeadTime + CurrentTime);
            }


			animSliderRect = GUILayoutUtility.GetLastRect();



			return animSliderRect;
		}


        /// <summary>
        /// GUI描画
        /// </summary>
        public void DrawTime(EditorWindow window, System.Action<AnimationClip> callback) {
            using (new GUILayout.HorizontalScope()) {
                EditorGUI.BeginChangeCheck();
                Clip = (AnimationClip)EditorGUILayout.ObjectField("Animation", Clip, typeof(AnimationClip), true);
                if (EditorGUI.EndChangeCheck()) {
                    callback(Clip);

                    if (Clip != null) {
                        MaxTime = Clip.length;
                    }
                }
                MaxTime = EditorGUILayout.FloatField(MaxTime, GUILayout.Width(60f));
                //ボタン押されたらclipの長さでリセットする
                if(GUILayout.Button("fixup", GUILayout.Width(60f))){
                    MaxTime = Clip.length;
                }

            }

            CurrentTime = EditorGUILayout.FloatField("Current Time:", CurrentTime);
        }


    }
}
