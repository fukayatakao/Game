using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class StrategyAlternativeMenu {
		//シングルトン
		private static StrategyAlternativeMenu instance_;
		public static StrategyAlternativeMenu I {
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
		private StrategyAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(StrategyMain main) {
			instance_ = new StrategyAlternativeMenu();
			instance_.OpenImpl(main);
		}

		/// <summary>
		/// ウィンドウを作る
		/// </summary>
		private void OpenImpl(StrategyMain main) {
			//インスタンスだけ作って非表示にする
			{
				InvaderLocationMenu menu = DebugWindowManager.Open<InvaderLocationMenu>();
				menu.Init(main);
				menu.Hide();
				window_.Add(menu);
			}

			{
				StrategyMainMenu menu = DebugWindowManager.Open<StrategyMainMenu>();
				menu.Hide();
				menu.Init(main.HaveStrategyBoard);
				window_.Add(menu);

			}

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
		public static void Close() {
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
