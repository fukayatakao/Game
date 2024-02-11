using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 建物メニュー下部の表示ボタン
	/// </summary>
	public enum FacilityMenuState {
		None,					//なし
		Default,				//通常時表示(建物の再配置,撤去)
		Build,                  //生成時表示(建物の配置確定,生成キャンセル)
		ReLocate,               //再配置時表示(建物の再配置決定,キャンセル)
	}



	/// <summary>
	/// 代替GUIの管理クラス
	/// </summary>
	public class TownAlternativeMenu {
		//シングルトン
		private static TownAlternativeMenu instance_;
		public static TownAlternativeMenu I {
			get {
				return instance_;
			}
		}

		//管理下にあるウィンドウのリスト
		TownMenu window_;
		public TownMenu BuildFacilityMenu { get { return window_; } }

		ChainInfoMenu chainInfoMenu_;

		List<DebugWindow> subWindow_ = new List<DebugWindow>();

		HeaderMenu headerMenu_;
		TransitMenu transitMenu_;

		TownhallMenu townhallMenu_;
		FactoryMenu factoryMenu_;
		ResidenceMenu residenceMenu_;
		StorageMenu storageMenu_;
		MarketMenu marketMenu_;
		ServiceMenu serviceMenu_;

		MessageSystem.Receptor receptor_;

		//これはいずれ外に出す
		Stack<DebugWindow> stackMainMenu_ = new Stack<DebugWindow>();
		public void Init(DebugWindow window) {
			stackMainMenu_.Push(window);
		}

		public void PushMenu(DebugWindow window) {
			//現在表示されてるメニューを非表示にする
			stackMainMenu_.Peek().Hide();
			//新しいウィンドウをPushして表示
			stackMainMenu_.Push(window);
			window.Show();
		}
		public void PopMenu() {
			stackMainMenu_.Pop().Hide();
			stackMainMenu_.Peek().Show();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private TownAlternativeMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}

		/// <summary>
		/// インスタンスを作って開く
		/// </summary>
		public static void Open(TownMain main) {
			instance_ = new TownAlternativeMenu();
			instance_.OpenImpl(main);
		}
		/// <summary>
		/// ウィンドウを作る
		/// </summary>
		private void OpenImpl(TownMain main) {
			window_ = DebugWindowManager.Open<TownMenu>();
			Init(window_);

			headerMenu_ = DebugWindowManager.Open<HeaderMenu>();
			headerMenu_.Init(main.CenterTownhall);

			transitMenu_ = DebugWindowManager.Open<TransitMenu>();

			townhallMenu_ = DebugWindowManager.Open<TownhallMenu>();
			townhallMenu_.Hide();
			factoryMenu_ = DebugWindowManager.Open<FactoryMenu>();
			factoryMenu_.Hide();
			residenceMenu_ = DebugWindowManager.Open<ResidenceMenu>();
			residenceMenu_.Hide();
			storageMenu_ = DebugWindowManager.Open<StorageMenu>();
			storageMenu_.Hide();
			marketMenu_ = DebugWindowManager.Open<MarketMenu>();
			marketMenu_.Hide();
			serviceMenu_ = DebugWindowManager.Open<ServiceMenu>();
			serviceMenu_.Hide();

			chainInfoMenu_ = DebugWindowManager.Open<ChainInfoMenu>();
			chainInfoMenu_.Init(main);
			chainInfoMenu_.Hide();

			subWindow_.Add(townhallMenu_);
			subWindow_.Add(factoryMenu_);
			subWindow_.Add(residenceMenu_);
			subWindow_.Add(storageMenu_);
			subWindow_.Add(marketMenu_);
			subWindow_.Add(serviceMenu_);

		}

		public void HideSub() {
			for (int i = 0, max = subWindow_.Count; i < max; i++) {
				subWindow_[i].Hide();
			}
		}

		/// <summary>
		/// 施設用メニューを開く
		/// </summary>
		public DebugWindow OpenMenu<T>(T facility, FacilityMenuState state) {
			HideSub();
			switch (facility) {
			case Townhall townhall:
				townhallMenu_.Show();
				townhallMenu_.Init(townhall, state);
				return factoryMenu_;
			case Factory factory:
				factoryMenu_.Show();
				factoryMenu_.Init(factory, state);
				return factoryMenu_;
			case Market market:
				marketMenu_.Show();
				marketMenu_.Init(market, state);
				return marketMenu_;
			case Residence residence:
				residenceMenu_.Show();
				residenceMenu_.Init(residence, state);
				return residenceMenu_;
			case Storage storage:
				storageMenu_.Show();
				storageMenu_.Init(storage, state);
				return storageMenu_;
			case Service service:
				serviceMenu_.Show();
				serviceMenu_.Init(service, state);
				return serviceMenu_;
			default:
				Debug.LogError("window type error:" + typeof(T));
				return null;
			}
		}

		public void OpenChainInfoMenu() {
			chainInfoMenu_.Show();
		}

		/// <summary>
		/// デバッグメニューを閉じたときの再表示対策
		/// </summary>
		public void Show() {
			//window_.Reset(true);
			window_.Show();
			headerMenu_.Show();
			transitMenu_.Show();
		}


		/// <summary>
		/// メニューを隠す
		/// </summary>
		public void Hide() {
			//window_.Reset(false);
			window_.Hide();
			headerMenu_.Hide();
			transitMenu_.Hide();
			HideSub();
		}

		/// <summary>
		/// 閉じる
		/// </summary>
		public static void Close() {
			instance_.CloseImpl();
			instance_ = null;
		}
		/// <summary>
		/// 閉じる
		/// </summary>
		private void CloseImpl() {
			DebugWindowManager.Close(transitMenu_);
			DebugWindowManager.Close(headerMenu_);
			DebugWindowManager.Close(window_);
			for (int i = 0, max = subWindow_.Count; i < max; i++) {
				DebugWindowManager.Close(subWindow_[i]);
			}
		}
	}
#endif
}
