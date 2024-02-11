using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if UNITY_EDITOR
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class DummyAlternativeMenu {
		//シングルトン
		private static DummyAlternativeMenu instance_;
		public static DummyAlternativeMenu I {
			get {
				return instance_;
			}
		}

		//管理下にあるウィンドウのリスト
		List<DebugWindow> window_ = new List<DebugWindow>();

		MessageSystem.Receptor receptor_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private DummyAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(MockBattleMain main) {
			instance_ = new DummyAlternativeMenu();
			instance_.OpenImpl(main);
		}

		/// <summary>
		/// ウィンドウを作る
		/// </summary>
		private void OpenImpl(MockBattleMain main) {
			//window_.Add(DebugWindowManager.Open<ChangeUnitMenu>());
			var timer = DebugWindowManager.Open<DisplayTime>();
			timer.Init(main.HaveTimer);
			window_.Add(timer);

		}

		/// <summary>
		/// デバッグメニューを閉じたときの再表示対策
		/// </summary>
		public void Show() {
			for(int i = 0, max = window_.Count; i < max; i++) {
				window_[i].Show();
			}
		}


		/// <summary>
		/// メニューを隠す
		/// </summary>
		public void Hide() {
			for (int i = 0, max = window_.Count; i < max; i++) {
				window_[i].Hide();
			}
		}

		/// <summary>
		/// 閉じてインスタンス破棄
		/// </summary>
		public static void Close()
		{
			if (instance_ == null)
				return;
			instance_.CloseImpl();
			instance_ = null;
		}
		/// <summary>
		/// 閉じてインスタンス破棄
		/// </summary>
		private void CloseImpl() {
			for (int i = 0, max = window_.Count; i < max; i++) {
				DebugWindowManager.Close(window_[i]);
			}
		}
	}
#endif
}
