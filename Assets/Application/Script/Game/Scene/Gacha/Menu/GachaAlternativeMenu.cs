using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class GachaAlternativeMenu {
		//シングルトン
		private static GachaAlternativeMenu instance_;
		public static GachaAlternativeMenu I {
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
		private GachaAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(GachaMain main) {
			instance_ = new GachaAlternativeMenu();
			instance_.OpenImpl(main);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		private void OpenImpl(GachaMain main) {
			//インスタンスだけ作って非表示にする
			var menu = DebugWindowManager.Open<CharaCreate>();
			menu.Init(main);
			window_.Add(menu);

			var result = DebugWindowManager.Open<CharaStatus>();
			result.Hide();
			window_.Add(result);

			var all = DebugWindowManager.Open<CharaAll>();
			all.Init();
			window_.Add(all);

		}
		/// <summary>
		/// デバッグメニューを閉じたときの再表示対策
		/// </summary>
		public void Show() {
			for (int i = 0, max = window_.Count; i < max; i++) {
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
