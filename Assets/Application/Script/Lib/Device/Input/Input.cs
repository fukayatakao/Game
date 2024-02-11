

//namespace Project.Lib {
#if INPUT_WRAPPER
	/// <summary>
	/// UnityのInputをラッピング
	/// </summary>
	/// <remarks>
	/// 入力情報を記録・再生する処理を挟み込む等で使う予定
	/// </remarks>
	public class Input{
		public static int touchCount{ get { return UseNativeInput ? UnityEngine.Input.touchCount : CurrentRecord.touchCount; } }
		public static Touch[] touches{ get { return UseNativeInput ? UnityEngine.Input.touches : CurrentRecord.touches; } }

		public static Touch GetTouch(int index){
			return UseNativeInput ? UnityEngine.Input.GetTouch (index) : CurrentRecord.touches[index];
		}


		//@note マウスの操作系は記録時点でタッチ操作に偽装して記録するので記録データから再生する処理は不要
		public static Vector3 mousePosition{get{return UseNativeInput ? UnityEngine.Input.mousePosition : CurrentRecord.mousePosition;}}
		public static bool GetMouseButtonDown(int index){
			return UseNativeInput ? UnityEngine.Input.GetMouseButtonDown (index) : CurrentRecord.mousePhase == TouchPhase.Began;
		}
		public static bool GetMouseButton(int index){
			return UseNativeInput ? UnityEngine.Input.GetMouseButton (index) : CurrentRecord.mousePhase == TouchPhase.Moved;
		}
		public static bool GetMouseButtonUp(int index){
			return UseNativeInput ? UnityEngine.Input.GetMouseButtonUp (index) : CurrentRecord.mousePhase == TouchPhase.Ended;
		}


		//情報量が多くなりすぎるので入力キー情報は記録しない
		public static bool GetKey(KeyCode key){
			return UnityEngine.Input.GetKey (key);
		}
		public static bool GetKeyDown(KeyCode key){
			return UnityEngine.Input.GetKeyDown (key);
		}
		public static bool GetKeyUp(KeyCode key){
			return UnityEngine.Input.GetKeyUp (key);
		}

        public static bool GetKey(string key){
			return UnityEngine.Input.GetKey (key);
		}

        public static bool GetKeyDown(string key) {
            return UnityEngine.Input.GetKeyDown(key);
        }

        public static bool GetButton(string name){
			return UnityEngine.Input.GetButton (name);
		}

        public static bool GetButtonDown(string key) {
            return UnityEngine.Input.GetButtonDown(key);
        }

        public static Vector3 acceleration{ get { return UnityEngine.Input.acceleration; } }

		public static float GetAxis(string axisName){
			return UnityEngine.Input.GetAxis (axisName);
		}

		public static IMECompositionMode imeCompositionMode{ get { return UnityEngine.Input.imeCompositionMode; } set { UnityEngine.Input.imeCompositionMode = value; } }

		public static Vector2 compositionCursorPos{ get { return UnityEngine.Input.compositionCursorPos; } set { UnityEngine.Input.compositionCursorPos = value; } }

		public static string inputString{ get { return UnityEngine.Input.inputString; } }

		public static string compositionString{ get { return UnityEngine.Input.compositionString; } }

		public static bool UseNativeInput = true;
		public static InputLog.Record CurrentRecord;
	}

#endif



//}


