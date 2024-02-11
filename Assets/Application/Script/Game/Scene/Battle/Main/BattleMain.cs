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
	public class BattleMain : MonoBehaviour {
		//戻るときの遷移先。ちょっと置き場所は考える。
		public static SceneTransition.SceneType PrevScene;

		//ユーザーがボタンを推したときのみ時間が進むモード
		[System.NonSerialized]
		public bool IsChronos;
#if MANIPULATE_TIME
		public const string TimesKey = "times";
		public const string LoopKey = "loop";

		//倍速値
		public int Times = 1;
		//ループ回数
		public int Loop = 1;
		public int Count = 0;
#endif
		[SerializeField]
		private int randomSeed_ = 999;

//インスタンス管理
#region Assembly
		//Entity集合クラス
		AssemlyManager assemblyManager_ = new AssemlyManager(
			new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
				BulletAssembly.CreateInstance,
				CharacterAssembly.CreateInstance,
				SquadAssembly.CreateInstance,
				PlatoonAssembly.CreateInstance,

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
		private FieldEntity stageField_ = null;
		public FieldEntity StageField{ get { return stageField_; } }
		//カメラ
		private CameraEntity mainCamera_ = null;
		public CameraEntity MainCamera{ get { return mainCamera_; } }
        //ライト
        private LightEntity directionalLight_ = null;
        public LightEntity DirectionalLight { get { return directionalLight_; } }
		//キャラクター関連
		PlatoonEntity[] platoon_ = new PlatoonEntity[(int)Power.Max];
		public PlatoonEntity[] Platoon { get { return platoon_; } }
#endregion

//ユーザー制御系のインスタンス
#region UserControl
		CharacterControl characterControl_;
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
			Opening,        // 入場演出
			Ready,			// 開始待ち
			Main,			// ゲームメイン
			Special,		// スキル演出中

			Max,
		}
		// ステート管理
		private StateMachine<BattleMain> haveState_ = null;
		public StateMachine<BattleMain> HaveState{get{ return haveState_; } }


#endregion
		private BattlePhaseBonus phaseBonus_;
		public BattlePhaseBonus PhaseBonus { get { return phaseBonus_; } }

		private BattleReport report_;
		public BattleReport Report { get { return report_; } }

		private BattleTimer haveTimer_;
		public BattleTimer HaveTimer { get { return haveTimer_; } }

		private BattleConstConfig const_;
		//本体のTransform
		protected Transform cacheTrans_;
		public Transform CacheTrans { get { return cacheTrans_; } }

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
			haveState_ = new StateMachine<BattleMain>((int)State.Max);
			haveState_.Register<StateBattleNone>(gameObject, (int)State.None);
			haveState_.Register<StateBattleDownload>(gameObject, (int)State.Download);
			haveState_.Register<StateBattleInitialize>(gameObject, (int)State.Initialize);
			haveState_.Register<StateBattleOpening>(gameObject, (int)State.Opening);
			haveState_.Register<StateBattleReady>(gameObject, (int)State.Ready);
			haveState_.Register<StateBattleMain>(gameObject, (int)State.Main);
			haveState_.Register<StateBattleSpecial>(gameObject, (int)State.Special);

            haveState_.SetFirstState((int)State.None);
			ChangeState(state);
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		private void Awake(){
#if MANIPULATE_TIME
			Times = PlayerPrefs.GetInt(TimesKey, Times);
			Loop = PlayerPrefs.GetInt(LoopKey, Loop);
#endif

#if DEVELOP_BUILD
			//直接シーンしていで起動された場合の処理
			EntryPoint.DirectBoot(EntryPoint.BootScene.Battle, () => { DebugBattleMainCmd.CreateAsync(new DebugBattleMainRequest(), SceneTransition.Transition); });

			BattleDebugKeyControl.Create(this);
			BattleDebugRoot.Add();
#endif
			AddressableChecker.CheckPoint("Battle Start");
			//乱数の種がマイナスの場合は時間で初期化
			DeterminateRandom.SetSeed(randomSeed_);

			MessageSystem.Create(new BattleMessageSetting());

			cacheTrans_ = transform;
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);

			//ユーザー操作の排他制御（他の制御より前にインスタンス作る）
			userOperation_ = new UserOperation((int)MessageGroup.TouchControl);

			//定数
			const_ = gameObject.GetComponent<BattleConstConfig>();
			//時間帯ボーナス
			phaseBonus_ = MonoPretender.Create<BattlePhaseBonus>(gameObject);
			//レポート
			report_ = MonoPretender.Create<BattleReport>(gameObject);
			//タイマー
			haveTimer_ = MonoPretender.Create<BattleTimer>(gameObject);
			assemblyManager_.Create(cacheTrans_);

			BattleCollision.Initialize();
			InitState(State.Download);
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		private void OnDestroy() {
			ChangeState(State.None);
#if MANIPULATE_TIME
			//コマンドライン引数を渡すだけなので使ったら消す
			PlayerPrefs.DeleteKey(TimesKey);
			PlayerPrefs.DeleteKey(LoopKey);
#endif
			MonoPretender.Destroy(phaseBonus_);
			MonoPretender.Destroy(report_);
			MonoPretender.Destroy(haveTimer_);


			haveState_.UnRegisterAll();
			//Entityの破棄
			assemblyManager_.Destroy();
			MessageSystem.Destroy();

			//リソース解放
			ResourceCache.Clear();

#if DEVELOP_BUILD
			CleanupDebug();

			BattleDebugRoot.Remove();
#endif
			BattleSituation.Destroy();
			System.GC.Collect();
			AddressableChecker.CheckPoint("Battle End");

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
			for (int i = 1; i < Times; i++) {
				Execute();
				LateExecute();
				Time.Execute();
			}
#endif
		}

		private void Execute() {
			if (haveTimer_.IsEnable()) {
				haveTimer_.Execute();
				phaseBonus_.Execute();
			}

			MessageSystem.Execute();
			haveState_.Execute(this);
			//Entityの実行処理
			assemblyManager_.Execute();

			assemblyManager_.Evaluate();
		}

		private void LateExecute() {
			//移動を行う前のコリジョン計算
			BattleCollision.PreCalculate(CharacterAssembly.I.Current, CharacterAssembly.I.Count, stageField_);
			//Entityの実行後処理
			assemblyManager_.LateExecute();
			//移動を行った後のコリジョン計算
			BattleCollision.PostCalculate(CharacterAssembly.I.Current, CharacterAssembly.I.Count, stageField_);
			//弾との衝突判定
			BattleCollision.BulletCalculate(CharacterAssembly.I.Current, CharacterAssembly.I.Count, BulletAssembly.I.Current, BulletAssembly.I.Count);
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
			characterControl_ = new CharacterControl(mainCamera_, WorldCanvas.transform);

			//相手チームの参照をセット
			platoon_[(int)Power.Enemy].HaveBlackboard.Opponent = platoon_[(int)Power.Player];
			platoon_[(int)Power.Player].HaveBlackboard.Opponent = platoon_[(int)Power.Enemy];

			BattleCombat.Init(stageField_, phaseBonus_, haveTimer_);

#if DEVELOP_BUILD
			//@todo 置き場所審議
			CalculateDebug.Load();
			AttackAreaDebug.Load();

#if UNITY_EDITOR
			//キャラクターの評価判定関数をデバッグするためのコンポーネントを付ける
			for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterEntity entity = CharacterAssembly.I.Current[i];
				GameObject obj = new GameObject("EvaluateTest");
				obj.transform.SetParent(entity.CacheTrans);
				obj.AddComponent<EvaluateTest>().Create(entity);
			}
#endif
#endif
		}
#if DEVELOP_BUILD
		/// <summary>
		/// デバッグ機能セットアップ
		/// </summary>
		public virtual void SetupDebug() {
			//デバッグウィンドウで作った仮メニューを表示する
			BattleAlternativeMenu.Open(this);
			BattleDebugSetting.Setting();
			//キャラクター情報の表示対象選択処理
			CharacterInfo.CharacterSelect = new CharacterInfo.CharacterSelectControl(mainCamera_);
		}
		/// <summary>
		/// デバッグ機能クリーンアップ
		/// </summary>
		public virtual void CleanupDebug() {
			BattleAlternativeMenu.Close();
		}
#endif

		/// <summary>
		/// ユーザー操作処理
		/// </summary>
		public void ExecUserContorl() {
			characterControl_.Execute();
			//操作の振り分け
			userOperation_.Execute();
		}

		/// <summary>
		/// 演出後のゲーム開始準備
		/// </summary>
		public void GameReady() {
			MainCamera.ChangeControlView(true);
			userOperation_.SetDefault((int)OperationPriority.CameraViewing);
		}
		/// <summary>
		/// バトル開始
		/// </summary>
		public void GameStart() {
			for (int i = 0, max = PlatoonAssembly.I.Count; i < max; i++) {
				PlatoonEntity entity = PlatoonAssembly.I.Current[i];
				entity.Enable = true;
			}
			SetAIEnable(true);
#if DEVELOP_BUILD
			//AIの設定はゲーム開始で変更されるのでもう一度デバッグ設定を呼び出す
			BattleDebugSetting.SettingAI();
#endif

			report_.Init(this);
			report_.RecStart();
		}
		/// <summary>
		/// バトル終了時処理
		/// </summary>
		public virtual void GameFinish() {
			report_.RecFinish();
#if UNITY_EDITOR
			report_.Report();
#endif
			SetAIEnable(false);
#if RUN_BACKGROUND
			Count++;
			if (Count >= Loop) {
				EditorApplication.Exit(0);
			} else {
				SceneTransition.ChangeBattle();
			}
#endif
		}

		/// <summary>
		/// AIの動作設定
		/// </summary>
		public void SetAIEnable(bool flag) {
			platoon_[(int)Power.Player].HaveThink.Enable = false;
			platoon_[(int)Power.Enemy].HaveThink.Enable = flag;

			for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterAssembly.I.Current[i].HaveThink.Enable = flag;
			}
		}
		/// <summary>
		/// フィールド生成
		/// </summary>
		public async Task CreateFieldEntity() {
			Mst.MstFieldData data = Mst.BaseDataManager.GetList<Mst.MstFieldData>()[BattleSituation.I.stageId];
			stageField_ = await FieldAssembly.I.CreateAsync(data.StageName, data.SkyName);
			stageField_.InitData(data);
		}

		/// <summary>
		/// カメラ生成
		/// </summary>
		public async Task CreateCameraEntity() {
			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.BATTLE_CAMERA_PREFAB);
			mainCamera_.ControlSetting.SetLengthMinMax(20f, 45f);
			mainCamera_.ControlSetting.SetAzimthMinMax(-120f, 120f);
			mainCamera_.ControlSetting.Init(30f, 45f, 90f);
			mainCamera_.LimitTargetView(-stageField_.Width * 0.5f, stageField_.Width * 0.5f, -stageField_.Depth * 0.5f, stageField_.Depth * 0.5f);

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
		public virtual async Task CreatePlayerCharacterEntity() {
			int index = (int)Power.Player;
			platoon_[index] = await PlatoonAssembly.I.CreateAsync("Player", "Platoon/AI/basic", BattleSituation.I.invader);
			platoon_[index].InitUI(mainCamera_, FloatUICanvas);
			platoon_[index].InitParam(Power.Player, stageField_.Width, stageField_.CenterDepth, stageField_.TerritoryDepth);
		}

		/// <summary>
		/// キャラクター生成
		/// </summary>
		public virtual async Task CreateEnemyCharacterEntity(){
			int index = (int)Power.Enemy;
			platoon_[index] = await PlatoonAssembly.I.CreateAsync("Enemy", BattleSituation.I.enemyAI, BattleSituation.I.defender);
			platoon_[index].InitUI(mainCamera_, FloatUICanvas);
			platoon_[index].InitParam(Power.Enemy, stageField_.Width, stageField_.CenterDepth, stageField_.TerritoryDepth);
        }


#if UNITY_EDITOR
		void OnDrawGizmos() {
			if (PlatoonAssembly.IsValid()) {
				for (int i = 0, max = PlatoonAssembly.I.Count; i < max; i++) {
					PlatoonAssembly.I.Current[i].DrawGizmos(stageField_.Width);
				}
			}
		}
#endif
	}

}
