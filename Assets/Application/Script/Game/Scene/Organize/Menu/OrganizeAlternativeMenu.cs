using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class OrganizeAlternativeMenu {
		//シングルトン
		private static OrganizeAlternativeMenu instance_;
		public static OrganizeAlternativeMenu I {
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
		private OrganizeAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(OrganizeMain main) {
			instance_ = new OrganizeAlternativeMenu();
			instance_.OpenImpl(main);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public void OpenImpl(OrganizeMain main) {
			//インスタンスだけ作って非表示にする
			var menu = DebugWindowManager.Open<CharaMenu>();
			window_.Add(menu);

			var info = DebugWindowManager.Open<PlatoonMenu>();
			info.Init(main);
			window_.Add(info);

			var system = DebugWindowManager.Open<OrganizeSystemMenu>();
			system.Init(main);
			window_.Add(system);
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
