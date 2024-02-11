using Project.Account;
using Project.Lib;
using Project.Network;
using UnityEngine;

namespace Project.Game {
	public static class EntryPoint
	{
		static bool initialized_ = false;
		/// <summary>
		/// ゲーム起動時に呼ばれる処理
		/// </summary>
		public static void Boot() {
			//初期化済なら即終了
			if (initialized_)
				return;

#if DEVELOP_BUILD
			if ( ( Application.platform == RuntimePlatform.WindowsPlayer ) ||
				( Application.platform == RuntimePlatform.OSXPlayer ) ) {
				// 16:9画面
				Screen.SetResolution(1280, 720, false);
			}
#else
			// リリースビルド時には全ログ無効
			UnityEngine.Debug.unityLogger.logEnabled = false;
#endif

			// QualitySettings を Low に固定する
			QualitySettings.SetQualityLevel(1);

			// マルチタップを禁止する
			Input.multiTouchEnabled = true;
			// Don't Sync（垂直同期無し）
			QualitySettings.vSyncCount = 0;

			// エディターでバックグラウンド動作する様に
			Application.runInBackground = true;

			// ローディングの優先度を設定する
			Application.backgroundLoadingPriority = ThreadPriority.Normal;

#if MANIPULATE_TIME
			// 目標フレームレート
			Time.Setup();
#endif
		}

		/// <summary>
		/// シーンで共通のインスタンスを生成して初期化
		/// </summary>
		public static void Initialize() {
			//初期化済なら即終了
			if (initialized_)
				return;
			//システムに近いので早めに生成
			Gesture.Create();

			VirtualScreen.Create();

			SceneTransition.Initialize();

			Command.Config.ServerRoot["sim"] = "";
			Command.Config.UserAgent["sim"] = "";
			TimeControl.Initialize();

			//常駐リソースをロード
			ResourcePool.Init();
#if DEVELOP_BUILD
			//デバッグウィンドウメニュー
			DebugWindowManager.Create(typeof(DebugRootMenu));
			DebugWindowManager.OpenEvent = () => { CommonMessage.DebugMenuHide.Broadcast(); TimeControl.Lock(); };
			DebugWindowManager.CloseEvent = () => { CommonMessage.DebugMenuShow.Broadcast(); TimeControl.UnLock();  };


#endif
#if USE_OFFLINE
			//オフライン用の関数の関連付け
			OfflineManager.Initialize();
			var obj = new GameObject("DummyServer");
			obj.AddComponent<Project.Server.DummyServer>();
			GameObject.DontDestroyOnLoad(obj);

#endif
			initialized_ = true;
		}
#if DEVELOP_BUILD
		public enum BootScene {
			Title,
			Organize,
			Strategy,
			Battle,
			Town,
			Gacha,
		}


		public static BootScene FirstBoot = BootScene.Title;
		public static void DirectBoot(BootScene scene, System.Action callback) {
			//初期化済なら即終了
			if (initialized_)
			{
				callback();
				return;
			}
			FirstBoot = scene;
			//@note メイン以外から直接開始するときの対策
			Boot();
			Initialize();
			LoginManager.Login(() => {
				callback();
			});
		}
#endif
	}
}

