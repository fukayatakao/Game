using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
 	/// <summary>
	/// 通常ボタン
	/// </summary>
    public class ButtonNormal {
        /// <summary>
        /// UI表示
        /// </summary>
        public static void Draw(string text, System.Action action) {
            if (FitGUILayout.Button(text)) {
                action();
            }
        }

        public static bool Draw(string text) {
            return FitGUILayout.Button(text);
        }
    }

    /// <summary>
    /// 新規ウィンドウを開く
    /// </summary>
    public class ButtonOpenWindow {
        /// <summary>
        /// UI表示
        /// </summary>
        public static void Draw<T>(string text) where T : DebugWindow, new(){
            if (FitGUILayout.Button(text)) {
                DebugWindowManager.Open<T>();
            }
        }
    }
	/// <summary>
	/// メインウィンドウを切り替え
	/// </summary>
    public class ButtonChangeWindow {
        /// <summary>
        /// UI表示
        /// </summary>
        public static void Draw<T>(string text) where T : DebugWindow, new() {
            if (FitGUILayout.Button(text)) {
                DebugWindowManager.Change<T>();
            }
        }
    }

    /// <summary>
    /// on/offスイッチボタン
    /// </summary>
    public class ButtonSwitch {
		static readonly string[] ButtonText = new string[] { "on", "off" };

        /// <summary>
        /// UI表示
        /// </summary>
		public static bool Draw(bool enable, string caption, System.Action on=null, System.Action off=null) {
			if (!string.IsNullOrEmpty(caption)) {
				FitGUILayout.Label(caption);
			}
			using (new GUILayout.HorizontalScope()) {
				Color col = GUI.color;
				if (enable) {
					GUI.color = Color.magenta;
				}
				if (FitGUILayout.Button("on")) {
                    if(on != null)
    					on();
					enable = true;
				}
				GUI.color = col;
				if (!enable) {
					GUI.color = Color.cyan;
				}
				if(FitGUILayout.Button("off")) {
                    if (off != null)
                        off();
                    enable = false;
				}
				GUI.color = col;
			}

			return enable;
        }
        /// <summary>
        /// UI表示
        /// </summary>
        public static bool Draw(bool enable, string onCaption, string offCaption, System.Action on=null, System.Action off=null) {
	        string caption = enable ? onCaption : offCaption;
	        System.Action action = enable ? on : off;

	        Color col = GUI.color;
	        GUI.color = enable ? Color.magenta : Color.cyan;
	        if (FitGUILayout.Button(caption)) {
		        if(action != null)
			        action();
		        enable = !enable;
	        }
	        GUI.color = col;
	        return enable;
        }
    }

#endif
}


