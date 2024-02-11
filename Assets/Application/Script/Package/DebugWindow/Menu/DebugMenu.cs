using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// 階層付きのデバッグメニュー
	/// </summary>
	public class DebugMenu {
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(System.Func<string> cap, System.Action act) {
			return new DebugMenu() { dynamicCaption_ = cap, action_ = act };
		}
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(string cap, System.Action act) {
			return new DebugMenu() { caption_ = cap, action_ = act };
		}
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(System.Func<string> cap, params DebugMenu[] child) {
			return new DebugMenu() { dynamicCaption_ = cap, child_ = new List<DebugMenu>(child) };
		}
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(string cap, params DebugMenu[] child) {
			return new DebugMenu() { caption_ = cap, child_ = new List<DebugMenu>(child) };
		}
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(System.Func<string> cap, System.Type window) {
			return new DebugMenu() { dynamicCaption_ = cap, window_ = window };
		}
		/// <summary>
		/// インスタンス作成用関数
		/// </summary>
		public static DebugMenu Item(string cap, System.Type window) {
			return new DebugMenu() { caption_ = cap, window_ = window };
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private DebugMenu() {
			//外部から使わない
		}
		//見出し
		protected string caption_;

		protected System.Func<string> dynamicCaption_;

		public string Caption { get { return dynamicCaption_ != null ? dynamicCaption_() : caption_; } }
		//実行処理、表示ウィンドウ、子階層はどれか一つのみ
		System.Action action_;
		System.Type window_;
		List<DebugMenu> child_;


		public void Add(DebugMenu menu) {
			child_.Add(menu);
		}
		public void Remove(DebugMenu menu) {
			child_.Remove(menu);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(ref DebugMenu current, History<DebugMenu> history, bool captionOnly) {
			DrawHeader(ref current, history, captionOnly);
			DrawBody(ref current, history);
		}
		/// <summary>
		/// 上部メニュー表示
		/// </summary>
		private void DrawHeader(ref DebugMenu current, History<DebugMenu> history, bool captionOnly) {
			CaptionLabel.Draw(Caption);
			if(!captionOnly) {
				using (new GUILayout.HorizontalScope()) {
					if (FitGUILayout.Button("<")) {
						current = history.Prev(current);
					}
					if (FitGUILayout.Button(">")) {
						current = history.Next(current);
					}
				}
			}
		}
		/// <summary>
		/// 本体表示
		/// </summary>
		protected void DrawBody(ref DebugMenu current, History<DebugMenu> history) {
			if (current.child_ == null)
				return;
			for (int i = 0, max = current.child_.Count; i < max; i++) {
				DebugMenu menu = current.child_[i];
				//ボタン表示
				if (FitGUILayout.Button(menu.Caption)) {
					//実行関数を登録していた場合はそれを実行
					if (menu.action_ != null) {
						menu.action_();
					//windowクラスの型が設定済なら開く
					}else if (menu.window_ != null) {
						DebugWindowManager.Open(menu.window_);
					//それ以外は階層ボタン
					} else {
						history.RecordHistory(current);
						current = menu;
					}
					break;
				}
			}

		}
	}
	/// <summary>
	/// 階層付きのデバッグメニューの描画クラス
	/// </summary>
	public class DebugMenuDrawer {

		public bool CaptionOnly;

		//GUI表示切替の履歴
		History<DebugMenu> history_ = new History<DebugMenu>();
		DebugMenu root_;
		//現在表示中のGUI
		DebugMenu current_;

		const float BOTTOM_OFFSET = 16f;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DebugMenuDrawer(DebugMenu root) {
			root_ = root;
			current_ = root;
		}
		/// <summary>
		/// メニューを追加
		/// </summary>
		public void Add(DebugMenu menu, DebugMenu parent = null) {
			DebugMenu node = parent;
			if(node == null) {
				node = root_;
			}
			node.Add(menu);
		}
		/// <summary>
		/// メニューを削除
		/// </summary>
		public void Remove(DebugMenu menu, DebugMenu parent = null) {
			DebugMenu node = parent;
			if (node == null) {
				node = root_;
			}
			node.Remove(menu);
		}

		public void Draw() {
			current_.Draw(ref current_, history_, CaptionOnly);
		}

	}

	//組み方のサンプル用
	public class DebugMenuSample : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		DebugMenuDrawer drawer_ = new DebugMenuDrawer(
			DebugMenu.Item("root",
				DebugMenu.Item("root1",
					DebugMenu.Item("child1",
						DebugMenu.Item("action3", () => { })
					),
					DebugMenu.Item("child2",
						DebugMenu.Item("action", () => { }),
						DebugMenu.Item("パフォーマンス", typeof(PerformanceWindow))
					)
				),
				DebugMenu.Item("root2",
					DebugMenu.Item("child2",
						DebugMenu.Item("action2", () => { }),
						DebugMenu.Item("ログ", typeof(DebugLogWindow))
					)
				)
			)
		);


		public DebugMenuSample() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}
		protected override void Draw(int id) {
			drawer_.Draw();
		}


	}

#endif
}
