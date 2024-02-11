using Project.Lib;
using Project.Network;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {

	/// <summary>
	/// メイン処理
	/// </summary>
	public class StrategyMain : MonoBehaviour {
//インスタンス管理
#region Assembly
		//Entity集合クラス
		AssemlyManager assemblyManager_ = new AssemlyManager(
			new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
				PieceAssembly.CreateInstance,

				FieldAssembly.CreateInstance,
				LightAssembly.CreateInstance,

			}
		);
#endregion



//Entity関連
#region GameEntity

		// UI
		public Transform UIRoot { get; private set; }
        public Canvas WorldCanvas { get; private set; }
		//キャラクターの浮遊UI置き場
		public Transform FloatUICanvas { get; private set; }
		//フィールド
		//private FieldEntity stageField_ = null;
		//public FieldEntity stageField{ get { return stageField_; } }
		//カメラ
		private CameraEntity mainCamera_ = null;
		public CameraEntity MainCamera{ get { return mainCamera_; } }
        //ライト
        private LightEntity directionalLight_ = null;
        public LightEntity DirectionalLight { get { return directionalLight_; } }
#endregion

//ユーザー制御系のインスタンス
#region UserControl
		private UserOperation userOperation_;

#endregion


//ゲームのステート管理
#region GameState
		/// <summary>
		/// ステート管理用
		/// </summary>
		public enum State {
			None,			// 初期無効状態
			Download,		// 初期化前にデータをサーバからロード
			Initialize,     // 初期化処理
			Main,           // フリーカメラ

			InvaderLocation,
			Move,
			MoveToBattle,


			Max,
		}
		// ステート管理
		private StateMachine<StrategyMain> haveState_ = null;
		public StateMachine<StrategyMain> HaveState{get{ return haveState_; } }


#endregion
		//本体のTransform
		protected Transform cacheTrans_;
		public Transform CacheTrans { get { return cacheTrans_; } }

		private StrategyBoard strategyBoard_ = null;
		public StrategyBoard HaveStrategyBoard { get { return strategyBoard_; } }


		/// <summary>
		/// ステート変更
		/// </summary>
		public void ChangeState(State state){
			haveState_.ChangeState(this, (int)state);
		}
		/// <summary>
		/// 状態の登録
		/// </summary>
		private void InitState(State state) {
			haveState_ = new StateMachine<StrategyMain>((int)State.Max);
			haveState_.Register<StateStrategyNone>(gameObject, (int)State.None);
			haveState_.Register<StateStrategyDownload>(gameObject, (int)State.Download);
			haveState_.Register<StateStrategyInitialize>(gameObject, (int)State.Initialize);
			haveState_.Register<StateStrategyMain>(gameObject, (int)State.Main);

			haveState_.Register<StateStrategyInvaderLocation>(gameObject, (int)State.InvaderLocation);
			haveState_.Register<StateStrategyMove>(gameObject, (int)State.Move);
			haveState_.Register<StateStrategyMoveToBattle>(gameObject, (int)State.MoveToBattle);


			haveState_.SetFirstState((int)State.None);
			ChangeState(state);
		}

#if DEVELOP_BUILD
		/// <summary>
		/// バトルシーンをデバッグ起動されたときの事前処理
		/// </summary>
		protected virtual void DebugBoot() {
		}
#endif
		/// <summary>
		/// インスタンス生成
		/// </summary>
		private void Awake(){
#if DEVELOP_BUILD
			//直接シーンしていで起動された場合の処理
			EntryPoint.DirectBoot(EntryPoint.BootScene.Strategy, () => { StrategyMainCmd.CreateAsync(new StrategyMainRequest(), SceneTransition.Transition); });

			//BattleDebugControl.Create(this);
			StrategyDebugRoot.Add();
#endif

			MessageSystem.Create(new StrategyMessageSetting());

			cacheTrans_ = transform;
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);

			//ユーザー操作の排他制御（他の制御より前にインスタンス作る）
			userOperation_ = new UserOperation((int)MessageGroup.TouchControl);

			assemblyManager_.Create(cacheTrans_);

			InitState(State.Download);

#if DEVELOP_BUILD
			DebugBoot();
#endif
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		private void OnDestroy() {
			ChangeState(State.None);
			haveState_.UnRegisterAll();
			//Entityの破棄
			assemblyManager_.Destroy();
			MessageSystem.Destroy();

			//リソース解放
			ResourceCache.Clear();
			StrategySituation.Destroy();
#if DEVELOP_BUILD
			CleanupDebug();

			StrategyDebugRoot.Remove();

#endif
			System.GC.Collect();

		}
		/// <summary>
		/// 更新処理
		/// </summary>
		private void Update(){
			Execute();
		}
		/// <summary>
		/// 更新後処理
		/// </summary>
		private void LateUpdate(){
			LateExecute();
#if MANIPULATE_TIME
			Time.Execute();
#endif
		}

		private void Execute() {
			haveState_.Execute(this);
			//Entityの実行処理
			assemblyManager_.Execute();
			MessageSystem.Execute();

			assemblyManager_.Evaluate();
		}

		private void LateExecute() {
			//Entityの実行後処理
			assemblyManager_.LateExecute();
		}
		/// <summary>
		/// バトルシーンの初期化
		/// </summary>
		public void Initialize() {
			UIRoot = UnityUtil.InstantiateChild(null, "UI").transform;

			WorldCanvas = UnityUtil.InstantiateChild(UIRoot, ResourceCache.Load<GameObject>(ResourcesPath.UI_WORLD_CANVAS)).GetComponent<Canvas>();
			FloatUICanvas = UnityUtil.InstantiateChild(WorldCanvas.transform, "FloatUI").transform;
		}
		/// <summary>
		/// オブジェクト生成後に行う初期化
		/// </summary>
		/// <returns></returns>
		public void InitializeLater() {
			// Entityの実行状態への移行
			assemblyManager_.Flush();
		}
#if DEVELOP_BUILD
		/// <summary>
		/// デバッグ機能セットアップ
		/// </summary>
		public void SetupDebug() {
			//デバッグウィンドウで作った仮メニューを表示する
			StrategyAlternativeMenu.Open(this);
			//StrategyDebugSetting.Setting();
		}
		/// <summary>
		/// デバッグ機能クリーンアップ
		/// </summary>
		public void CleanupDebug() {
			StrategyAlternativeMenu.Close();
		}

#endif

		/// <summary>
		/// ユーザー操作処理
		/// </summary>
		public void ExecUserContorl() {
			//操作の振り分け
			userOperation_.Execute();
		}

		/// <summary>
		/// フィールド生成
		/// </summary>
		public async Task CreateFieldEntity() {
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);

		}

		/// <summary>
		/// カメラ生成
		/// </summary>
		public async Task CreateCameraEntity() {
			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.STRATEGY_CAMERA_PREFAB);
			mainCamera_.ControlSetting.SetLengthMinMax(3f, 60f);
			mainCamera_.ControlSetting.SetAzimthMinMax(-120f, 120f);
			mainCamera_.ControlSetting.Init(20f, 45f, 0f);
			mainCamera_.LimitTargetView(-50f, 50f, -50f, 50f);

			mainCamera_.ChangeControlView(true);
			userOperation_.SetDefault((int)OperationPriority.CameraViewing);
			//3Dカメラ上で表示するキャンバスと関連付け
			WorldCanvas.worldCamera = mainCamera_.Camera;
		}

		/// <summary>
		/// ライト生成
		/// </summary>
		public async Task CreateLightEntity() {
			directionalLight_ = await LightAssembly.I.CreateAsync(ResourcesPath.LIGHT_DIRECTIONAL);
		}
		/// <summary>
		/// プレイヤーサイドのキャラクター生成
		/// </summary>
		public async Task CreatePieceEntity() {
			strategyBoard_ = new StrategyBoard();
			strategyBoard_.Init(StrategySituation.I.turn, StrategySituation.I.available);
			await strategyBoard_.LoadField(StrategySituation.I.mapId);
			await strategyBoard_.LocateInvader(StrategySituation.I.invader, StrategySituation.I.invaderLocation);
			await strategyBoard_.LocateDefender(StrategySituation.I.defender, StrategySituation.I.defenderLocation);

		}
	}

}
