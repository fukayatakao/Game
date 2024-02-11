using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class DebugLogWindow : DebugWindow {
        static readonly Vector2 mergin = new Vector2(0f, 0f);
        static readonly Vector2 size = new Vector2(1f, 0.3f);
        static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		Vector2 scrollPos_ = new Vector2();
		List<string> logText = new List<string>();

		bool showErrorLog_ = true;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugLogWindow() {
            base.Init(FitCommon.CalcRect(mergin, size, align));

            SetWindowTexture(TextureUtil.bgAlert, TextureUtil.bgAlert);
#if !UNITY_EDITOR
			SetAutoOpenErrorLog();
#endif
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			showErrorLog_ = FitGUILayout.Toggle(showErrorLog_, "エラーログ強制表示");

			scrollPos_ = FitGUILayout.BeginScrollView(scrollPos_);
			for (int i = 0, max = logText.Count; i < max; i++) {
				FitGUILayout.Label(logText[i], FitCommon.SMALL_FONT_SIZE);
			}
            FitGUILayout.EndScrollView();

		}
		/// <summary>
		/// エラーログの自動表示するかをセット
		/// </summary>
		public void SetAutoOpenErrorLog() {
			Application.logMessageReceivedThreaded += DisplayLog;
		}


		// ログコールバック
		public void DisplayLog(string logString, string stackTrace, LogType type) {
			lock (this) {
				/// ログ
				if (type == LogType.Log) {
					//logText.Add(logString + "\n" + stackTrace);
				}else if (type == LogType.Warning) {
					//logText.Add(logString + "\n" + stackTrace);
				}else {
					logText.Add(logString + "\n" + stackTrace);
                    if(showErrorLog_)
					    Show();
				}
			}
		}

	}
#endif
}
