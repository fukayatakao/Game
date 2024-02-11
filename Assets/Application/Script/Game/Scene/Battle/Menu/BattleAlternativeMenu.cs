using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class BattleAlternativeMenu {
		//シングルトン
		private static BattleAlternativeMenu instance_;
		public static BattleAlternativeMenu I {
			get {
				return instance_;
			}
		}

		//管理下にあるウィンドウのリスト
		List<DebugWindow> window_ = new List<DebugWindow>();
		DebugWindow endWindow_;

		MessageSystem.Receptor receptor_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private BattleAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(BattleMain main) {
			instance_ = new BattleAlternativeMenu();
			instance_.OpenImpl(main);
		}

		/// <summary>
		/// ウィンドウを作る
		/// </summary>
		private void OpenImpl(BattleMain main) {
			int playerTeamIndex = (int)Power.Player;

			for (int i = 0; i < (int)Abreast.MAX; i++) {
				SquadMenu menu = DebugWindowManager.Open<SquadMenu>(true);
				menu.Init(main, playerTeamIndex, i);
				menu.SetPosition(new Vector2(0.9f, i * 0.1f));
				if (i == 0) {
					menu.SetSize(new Vector2(0.25f, 0.1f));
				} else {
					menu.SetSize(new Vector2(0.2f, 0.1f));
				}
				window_.Add(menu);
			}

			var swapMenu = DebugWindowManager.Open<SwapMenu>();
			swapMenu.Init(main.Platoon[(int)Power.Player]);
			window_.Add(swapMenu);

			var specialPointMenu = DebugWindowManager.Open<SpecialPointMenu>();
			specialPointMenu.Init(main);
			window_.Add(specialPointMenu);

			var elapse = DebugWindowManager.Open<ElapseMenu>();
			elapse.Init(main.PhaseBonus);
			window_.Add(elapse);

			var timer = DebugWindowManager.Open<DisplayTime>();
			timer.Init(main.HaveTimer);
			window_.Add(timer);

			var enemyMenu = DebugWindowManager.Open<EnemySquadMenu>();
			enemyMenu.Init(main);
			window_.Add(enemyMenu);


			//インスタンスだけ作って非表示にする
			endWindow_ = DebugWindowManager.Open<BattleEndMessage>();
			endWindow_.Hide();
		}

		public CharacterInfo OpenCharacterInfo()
		{
			var info = (DebugWindowManager.Open(typeof(CharacterInfo)) as CharacterInfo);
			window_.Add(info);
			return info;
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
			DebugWindowManager.Close(endWindow_);

		}
	}
#endif
}
