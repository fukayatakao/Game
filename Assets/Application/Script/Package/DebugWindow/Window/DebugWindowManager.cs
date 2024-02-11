using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class DebugWindowManager : MonoBehaviour {
		static DebugWindow current_;

		static List<DebugWindow> windowList_ = new List<DebugWindow>();

		static bool enable_ = false;

		static GameObject gameObject_;

		public static System.Action OpenEvent = () => { };
		public static System.Action CloseEvent = () => { };

		private static GUISkin skin_;
		/// <summary>
		/// インスタンス作成
		/// </summary>
		public static void Create(System.Type rootWindow)
		{
			skin_ = Resources.Load("DebugGUISkin") as GUISkin;

			if (gameObject_ != null)
				return;
			gameObject_ = new GameObject("DebugWindow");
			gameObject_.AddComponent<DebugWindowManager>().Init(rootWindow);
			GameObject.DontDestroyOnLoad(gameObject_);
		}
		/// <summary>
		/// インスタンス作成
		/// </summary>
		public static void Destroy() {
			if (gameObject_ == null)
				return;

			GameObject.Destroy(gameObject_);
			Resources.UnloadAsset(skin_);
		}
		/// <summary>
		/// デバッグウィンドウ表示をonにする
		/// </summary>
		public static void SetActive(bool flag) {
			if (enable_ == flag)
				return;
            enable_ = flag;
			if (enable_) {
				OpenEvent();
				current_.Show();
			} else {
				for (int i = 0, max = windowList_.Count; i < max; i++) {
					windowList_[i].Hide();
				}
				CloseEvent();
			}
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public void Init(System.Type rootWindow) {
			windowList_.Clear();
			current_ = CreateWindow(rootWindow);

            //current_ = CreateWindow<DebugMainWindow>();
			//current_.Show();
			enable_ = false;
			//インスタンスを作ってすぐ非表示にして裏で持たせておく
			CreateWindow<DebugLogWindow>().Hide();
        }
        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown(2)) {
#else
			//マウスの中央ボタンor３点目のタッチでデバッグメニューを表示する
			if (Input.touchCount >= 3 && Input.touches[2].phase == TouchPhase.Began) {
#endif
				SetActive(!enable_);
			}
		}
        /// <summary>
        /// GUI表示
        /// </summary>
		private void OnGUI() {
#if !UNITY_EDITOR
	        //アプリビルドするとデフォルト設定ではフォントに日本語が含まれず文字化けするのでちゃんとフォントを設定したskinをセットする
	        //UnityEditorでは逆にfontの参照がはがれてエラーになることがあるので無効にする
	        GUI.skin = skin_;
#endif


#if UNITY_EDITOR
			if (FitCommon.IsResize()) {
				for (int i = 0, max = windowList_.Count; i < max; i++) {
					windowList_[i].ResizeWindow();
				}
			}
#endif
			for (int i = 0, max = windowList_.Count; i < max; i++) {
				windowList_[i].DrawWindow();
			}
		}
        /// <summary>
        /// 新規windowを表示
        /// </summary>
        public static T Open<T>(bool isDuplicate = false) where T : DebugWindow, new(){
			//重複しないウィンドウは既存のインスタンスを探してあったらそれを返す
			if(!isDuplicate){
				for (int i = 0; i < windowList_.Count; i++) {
					if (windowList_[i].GetType() == typeof(T) && windowList_[i].IsValid()) {
						return (T)windowList_[i];
					}
				}
			}
			T window = CreateWindow<T>();
			window.Show();
            return window;
		}
		/// <summary>
		/// window破棄
		/// </summary>
		public static void Close(DebugWindow window) {
			windowList_.Remove(window);
		}

		/// <summary>
		/// 現在のwindowと入れ替えで表示
		/// </summary>
		public static T Change<T>() where T : DebugWindow, new(){
			current_.Hide();

            T window = CreateWindow<T>();
            current_ = window;

			current_.Show();
            return window;
		}
		/// <summary>
		/// 新規ウィンドウ作成
		/// </summary>
		private static T CreateWindow<T>()  where T : DebugWindow, new(){
			//現在表示されていないインスタンスがあればそれを使う
			for (int i = 0; i < windowList_.Count; i++) {
				if (windowList_[i].GetType() == typeof(T) && !windowList_[i].IsValid()) {
					return (T)windowList_[i];
				}
			}
			//インスタンスがなければ生成
			T win = new T();
			windowList_.Add(win);
			return win;
		}

		/// <summary>
		/// 新規windowを表示
		/// </summary>
		public static DebugWindow Open(System.Type type, bool isDuplicate=false) {
			//重複しないウィンドウは既存のインスタンスを探してあったらそれを返す
			if (!isDuplicate) {
				for (int i = 0; i < windowList_.Count; i++) {
					if (windowList_[i].GetType() == type && windowList_[i].IsValid()) {
						return windowList_[i];
					}
				}
			}
			DebugWindow window = CreateWindow(type);
			window.Show();
			return window;
		}
		/// <summary>
		/// 新規ウィンドウ作成
		/// </summary>
		private static DebugWindow CreateWindow(System.Type type) {
            //現在表示されていないインスタンスがあればそれを使う
            for (int i = 0; i < windowList_.Count; i++) {
                if (windowList_[i].GetType() == type && !windowList_[i].IsValid()) {
                    return windowList_[i];
                }
            }

            //インスタンスがなければ生成
            DebugWindow win = (DebugWindow)System.Activator.CreateInstance(type);
            windowList_.Add(win);
            return win;
        }
		/// <summary>
		/// 新規windowを表示
		/// </summary>
		public static DebugWindow Open(DebugWindow window) {
			windowList_.Add(window);
			window.Show();
			return window;
		}


		/// <summary>
		/// 表示されているか
		/// </summary>
		public static bool IsShow<T>() {
			//現在表示されているインスタンスがあるかチェック
			for (int i = 0; i < windowList_.Count; i++) {
				if (windowList_[i].GetType() == typeof(T) && windowList_[i].IsValid()) {
					return true;
				}
			}

			return false;
		}
        /// <summary>
        /// デバッグウィンドウ全閉じ
        /// </summary>
        public static void Exit() {
			for (int i = 0, max = windowList_.Count; i < max; i++) {
				if (windowList_[i].IsValid()) {
					windowList_[i].Hide();
				}
			}
			enable_ = false;
		}
        /// <summary>
        /// タッチした位置がデバッグウィンドウ上になるか
        /// </summary>
        public static bool InWindowArea(Vector2 touchPos) {
			touchPos.y = Screen.height - touchPos.y;
			//現在表示されているインスタンスがあるかチェック
			for (int i = 0; i < windowList_.Count; i++) {
				//有効でないウィンドウは無視
				if (!windowList_[i].IsValid())
					continue;
				//矩形範囲外の場合は無視
				if (!windowList_[i].WindowRect.Contains(touchPos))
					continue;

				return true;
			}

			return false;
		}
	}
#endif
}

