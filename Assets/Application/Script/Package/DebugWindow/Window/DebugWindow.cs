using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public abstract class DebugWindow
	{
        private int id_ = 0;
		private bool enable_ = false;
		//idの採番用カウンター
		private static UniqueCounter counter_ = new UniqueCounter();

        //ウィンドウの実ピクセルサイズ
        private Rect originalRect_;
        private Rect windowRect_;
        public Rect WindowRect{ get{ return windowRect_; } }
        //ドラッグエリアの実ピクセルサイズ
        protected Rect dragRect_;

		Texture2D normalTex_ = null;
		Texture2D onNormalTex_ = null;

		private float offset_ = -1;

		protected System.Action ShowEvent = () => { };
		protected System.Action HideEvent = () => { };

		/// <summary>
		/// ウィンドウ表示に使うテクスチャをセット
		/// </summary>
		public void SetWindowTexture(Texture2D normal, Texture2D onNormal) {
			normalTex_ = normal;
			onNormalTex_ = onNormal;
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		protected void Init(Rect winRect, float drawWidth = float.MaxValue, float drawHeight = float.MaxValue) {
            //FitCommonのサイズ計算にVirtualScreenUtilが必要なので有効になっているかチェック
            Debug.Assert(VirtualScreen.IsValid(), "VirtualScreenUtil is not ready");
            //IDは最初の1回だけ採番する
            id_ = counter_.GetUniqueId();
			originalRect_ = winRect;
			windowRect_ = FitCommon.CalcRectSize(originalRect_);
			dragRect_ = FitCommon.CalcRectSize(new Rect(0f, 0f, drawWidth, drawHeight));
		}

		/// <summary>
		/// ウィンドウの位置を設定(左上基準の画面サイズ割合
		/// </summary>
		public void SetPosition(Vector2 pos) {
			pos.x = pos.x * Screen.width;
			pos.y = pos.y * Screen.height;
			windowRect_.position = pos;

			originalRect_ = FitCommon.CalcScreenRectSize(windowRect_);
		}
		/// <summary>
		/// ウィンドウの位置を設定(左上基準の画面サイズ割合
		/// </summary>
		public void SetPositionRaw(Vector2 pos) {
			windowRect_.position = pos;

			originalRect_ = FitCommon.CalcScreenRectSize(windowRect_);
		}

		/// <summary>
		/// ウィンドウのサイズを設定(画面サイズ割合
		/// </summary>
		public void SetSize(Vector2 size) {
			size.x = size.x * Screen.width;
			size.y = size.y * Screen.height;
			windowRect_.size = size;
			originalRect_ = FitCommon.CalcScreenRectSize(windowRect_);
		}

		/// <summary>
		/// 有効か
		/// </summary>
		public bool IsValid() {
			return enable_;
		}
        /// <summary>
        /// ウィンドウを表示する
        /// </summary>
		public void Show() {
			ShowEvent();
			enable_ = true;
		}
        /// <summary>
        /// ウィンドウを隠す
        /// </summary>
		public void Hide() {
			enable_ = false;
			HideEvent();
		}
        /// <summary>
        /// GUI表示処理
        /// </summary>
		public void DrawWindow() {
			//表示しない場合はここでreturn
			if (!enable_)
				return;

			GUIStyle style = new GUIStyle(GUI.skin.window);
           //ウィンドウを表示
			style.fontSize = FitCommon.CalcFontSize(FitCommon.SMALL_FONT_SIZE);
			//@note backgroundで使っているテクスチャ自体にαが入っていてテクスチャを変えないことには不透過にはできないらしい
			if (normalTex_ != null) {
				style.normal.background = normalTex_;
			}
			if (onNormalTex_ != null) {
				style.onNormal.background = onNormalTex_;
			}
			//GUI.color = new Color(0f, 1f, 0f, 200f) ;

			//ウィンドウ表示
			windowRect_ = GUI.Window(id_, windowRect_, DrawGUI, "", style);
            //画面外にウィンドウが出ないようにクランプする
            windowRect_ = ClampScreenPos(windowRect_);
		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected void DrawGUI(int windowID) {
			Draw(windowID);
			GUI.DragWindow(dragRect_);
			AutoReSize();
		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected abstract void Draw(int windowID);

		/// <summary>
		/// ウィンドウサイズを再計算
		/// </summary>
		public void ResizeWindow() {
			windowRect_ = FitCommon.CalcRectSize(originalRect_);
		}

		/// <summary>
		/// 縦幅の自動サイズ設定状態か
		/// </summary>
		public bool IsAutoResize(float offset = 16f) {
			return offset_ == -1;
		}
		/// <summary>
		/// 縦幅の自動サイズ設定
		/// </summary>
		public void SetAutoResize(float offset = 16f) {
			offset_ = offset;
		}
		/// <summary>
		/// 縦幅の自動サイズ設定解除
		/// </summary>
		public void UnSetAutoResize() {
			offset_ = -1;
		}
		/// <summary>
		/// 縦幅の自動調整
		/// </summary>
		private void AutoReSize() {
			if (offset_ < 0)
				return;
			//GUIが描画されたときだけ処理
			if (Event.current.type == EventType.Repaint) {
				Rect r = GUILayoutUtility.GetLastRect();
				windowRect_.yMax = windowRect_.yMin + r.yMax + FitCommon.CalcHeight(offset_);
			}
		}

		/// <summary>
		/// スクリーン内に収まるようにウィンドウの位置をクランプ
		/// </summary>
		private static Rect ClampScreenPos(Rect r) {
			if (r.x < 0) {
				r.x = 0;
			}
			if (r.x + r.width > Screen.width) {
				r.x = Screen.width - r.width;
			}
			if (r.y < 0) {
				r.y = 0;
			}
			if (r.y + r.height > Screen.height) {
				r.y = Screen.height - r.height;
			}

			return r;
		}
	}

#endif
}
