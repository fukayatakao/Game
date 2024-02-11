using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class TitleAlternativeMenu {
		//管理下にあるウィンドウのリスト
		List<DebugWindow> window_ = new List<DebugWindow>();
		//シングルトン
		private static TitleAlternativeMenu instance_;
		public static TitleAlternativeMenu I {
			get {
				return instance_;
			}
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		private TitleAlternativeMenu() {
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(TitleMain main) {
			instance_ = new TitleAlternativeMenu();
			//インスタンスだけ作って非表示にする
			var menu = DebugWindowManager.Open<SceneSelectMenu>();
			instance_.window_.Add(menu);
		}

		/// <summary>
		/// 閉じてインスタンス破棄
		/// </summary>
		public static void Close() {
			for(int i = 0, max = instance_.window_.Count; i < max; i++) {
				DebugWindowManager.Close(instance_.window_[i]);
			}

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
	}
#endif
}
