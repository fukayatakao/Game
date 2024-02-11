using UnityEngine;
using Project.Lib;
using Project.Network;


namespace Project.Game {
	/// <summary>
	/// メイン処理
	/// </summary>
	public partial class GachaMain : MonoBehaviour {
		public UnityEngine.UI.Image Image;
		public UnityEngine.UI.Text Text;

		//ゲームのステート管理
		#region GameState
		/// <summary>
		/// ステート管理用
		/// </summary>
		public enum State {
			None,           // 初期無効状態
			Download,       // 初期化前にデータをサーバからロード
			Initialize,     // 初期化処理
			Main,           // ゲームメイン
			Max,
		}
		// ステート管理
		private StateMachine<GachaMain> haveState_ = null;
		public StateMachine<GachaMain> HaveState { get { return haveState_; } }

		/// <summary>
		/// ステート変更
		/// </summary>
		public void ChangeState(State state) {
			haveState_.ChangeState(this, (int)state);
		}

#endregion

		//本体のTransform
		protected Transform cacheTrans_;
		public Transform CacheTrans { get { return cacheTrans_; } }
		// UI
		public Transform UIRoot { get; private set; }
		public Canvas WorldCanvas { get; private set; }


		/// <summary>
		/// 状態の登録
		/// </summary>
		private void InitState(State state) {
			haveState_ = new StateMachine<GachaMain>((int)State.Max);
			haveState_.Register<StateGachaNone>(gameObject, (int)State.None);
			haveState_.Register<StateGachaDownload>(gameObject, (int)State.Download);
			haveState_.Register<StateGachaInitialize>(gameObject, (int)State.Initialize);
			haveState_.Register<StateGachaMain>(gameObject, (int)State.Main);

			haveState_.SetFirstState((int)State.None);
			ChangeState(state);
		}


		/// <summary>
		/// インスタンス生成
		/// </summary>
		private void Awake() {
#if DEVELOP_BUILD
			EntryPoint.DirectBoot(EntryPoint.BootScene.Gacha, () => { GachaMainCmd.CreateAsync(new GachaMainRequest(), SceneTransition.Transition); });
#endif
			MessageSystem.Create(new GachaMessageSetting());

			cacheTrans_ = transform;
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);

			InitState(State.Download);
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		private void OnDestroy() {
			haveState_.UnRegisterAll();
			GachaSituationData.Destroy();
			MessageSystem.Destroy();

			//リソース解放
			ResourceCache.Clear();

#if DEVELOP_BUILD
			GachaAlternativeMenu.Close();
#endif
			System.GC.Collect();

		}
		/// <summary>
		/// 更新処理
		/// </summary>
		private void Update() {
			haveState_.Execute(this);
			//Entityの実行処理
			//assemblyManager_.LateExecute();
			MessageSystem.Execute();

			//assemblyManager_.Evaluate();
		}
		/// <summary>
		/// 更新後処理
		/// </summary>
		private void LateUpdate() {
			//Entityの実行後処理
			//assemblyManager_.LateExecute();
#if MANIPULATE_TIME
			Time.Execute();
#endif
		}


		/// <summary>
		/// シーンの初期化
		/// </summary>
		public void Initialize() {
			UIRoot = UnityUtil.InstantiateChild(null, "UI").transform;

			WorldCanvas = UnityUtil.InstantiateChild(UIRoot, ResourceCache.Load<GameObject>(ResourcesPath.UI_WORLD_CANVAS)).GetComponent<Canvas>();
		}
		/// <summary>
		/// オブジェクト生成後に行う初期化
		/// </summary>
		/// <returns></returns>
		public void InitializeLater() {
		}

#if DEVELOP_BUILD
		/// <summary>
		/// バトルシーンのデバッグ機能初期化
		/// </summary>
		public void InitializeDebug() {
			//デバッグウィンドウで作った仮メニューを表示する
			GachaAlternativeMenu.Open(this);
		}
#endif
	}
}
