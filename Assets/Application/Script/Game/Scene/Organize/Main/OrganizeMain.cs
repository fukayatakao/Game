using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using System.Threading.Tasks;
using Project.Network;
using System.Threading;

namespace Project.Game {
	/// <summary>
	/// メイン処理
	/// </summary>
	public partial class OrganizeMain : MonoBehaviour {
		public static OrganizeMainResponse TransitionData;
		//戻るときの遷移先。ちょっと置き場所は考える。
		public static SceneTransition.SceneType PrevScene;

//インスタンス管理
#region Assembly
		//Entity集合クラス
		AssemlyManager assemblyManager_ = new AssemlyManager(
			new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
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
		PlatoonEntity platoon_ = null;
		public PlatoonEntity Platoon { get { return platoon_; } }

#endregion

		//ユーザー制御系のインスタンス
#region UserControl
		CharacterSelectControl selectControl_;

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
			Main,			// ゲームメイン
			Max,
		}
		// ステート管理
		private StateMachine<OrganizeMain> haveState_ = null;
		public StateMachine<OrganizeMain> HaveState{get{ return haveState_; } }


#endregion

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
			haveState_ = new StateMachine<OrganizeMain>((int)State.Max);
			haveState_.Register<StateOrganizeNone>(gameObject, (int)State.None);
			haveState_.Register<StateOrganizeDownload>(gameObject, (int)State.Download);
			haveState_.Register<StateOrganizeInitialize>(gameObject, (int)State.Initialize);
			haveState_.Register<StateOrganizeMain>(gameObject, (int)State.Main);

			haveState_.SetFirstState((int)State.None);
			ChangeState(state);
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		private void Awake(){
#if DEVELOP_BUILD

			EntryPoint.DirectBoot(EntryPoint.BootScene.Organize, () => { OrganizeMainCmd.CreateAsync(new OrganizeMainRequest(), SceneTransition.Transition); });
			OrganizeDebugKeyControl.Create(this);
#endif
			AddressableChecker.CheckPoint("Organize Start");

			MessageSystem.Create(new OrganizeMessageSetting());

			cacheTrans_ = transform;
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);

			//ユーザー操作の排他制御（他の制御より前にインスタンス作る）
			userOperation_ = new UserOperation((int)MessageGroup.TouchControl);

			assemblyManager_.Create(cacheTrans_);

			CharacterAssembly.I.IsStock = false;

			InitState(State.Download);
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

#if DEVELOP_BUILD
			OrganizeAlternativeMenu.Close();
#endif
			System.GC.Collect();

			AddressableChecker.CheckPoint("Organize End");
		}
		/// <summary>
		/// 更新処理
		/// </summary>
		private void Update() {
			haveState_.Execute(this);
			//Entityの実行処理
			assemblyManager_.Execute();
			MessageSystem.Execute();

			assemblyManager_.Evaluate();
		}
		/// <summary>
		/// 更新後処理
		/// </summary>
		private void LateUpdate(){
			//Entityの実行後処理
			assemblyManager_.LateExecute();
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
			FloatUICanvas = UnityUtil.InstantiateChild(WorldCanvas.transform, "FloatUI").transform;
		}
		/// <summary>
		/// オブジェクト生成後に行う初期化
		/// </summary>
		/// <returns></returns>
		public void InitializeLater() {
			// Entityの実行状態への移行
			assemblyManager_.Flush();

			selectControl_ = new CharacterSelectControl(mainCamera_, WorldCanvas.transform);

			for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterAssembly.I.Current[i].HaveThink.Enable = true;
				CharacterAssembly.I.Current[i].SetupEdit();
			}


		}
#if DEVELOP_BUILD
		/// <summary>
		/// バトルシーンのデバッグ機能初期化
		/// </summary>
		public void InitializeDebug() {
			//デバッグウィンドウで作った仮メニューを表示する
			OrganizeAlternativeMenu.Open(this);

			OrganizeDebugSetting.Setting();
		}
#endif

		/// <summary>
		/// ユーザー操作処理
		/// </summary>
		public void ExecUserContorl() {
			selectControl_.Execute();
			//操作の振り分け
			userOperation_.Execute();
		}


		/// <summary>
		/// フィールド生成
		/// </summary>
		public async Task CreateFieldEntity(){
			Mst.MstFieldData data = Mst.MstFieldData.GetData(OrganizeSituationData.I.stageId);
			stageField_ = await FieldAssembly.I.CreateAsync(data.StageName, data.SkyName);
			stageField_.InitData(data);
		}
		/// <summary>
		/// カメラ生成
		/// </summary>
		public async Task CreateCameraEntity(){
			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.VIEW_CAMERA_PREFAB);
			mainCamera_.ControlSetting.SetLengthMinMax(3f, 30f);
			mainCamera_.ControlSetting.SetAzimthMinMax(120f, 240f);
			mainCamera_.ControlSetting.Init(15f, 55f, 180f);
			mainCamera_.LimitTargetView(-stageField_.Width * 0.25f, stageField_.Width * 0.25f, -stageField_.Depth * 0.5f, stageField_.Depth * 0.5f);
			mainCamera_.LookAtView(new Vector3(0f, 0f, 0f));

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
		public async Task CreatePlayerCharacterEntity(int index) {
			OrganizeSituationData.I.SelectPlatoon = index;

			platoon_ = await PlatoonAssembly.I.CreateAsync("Player", "Platoon/AI/basic", OrganizeSituationData.I.platoons[index]);
			platoon_.InitUI(mainCamera_, FloatUICanvas);
			platoon_.InitParam(Power.Player, stageField_.Width, stageField_.CenterDepth, stageField_.TerritoryDepth, false);

			platoon_.Enable = false;


			for (int i = 0, max = platoon_.Squads.Count; i < max; i++) {
				for(int j = 0, max2 = platoon_.Squads[i].Members.Count; j < max2; j++) {
					CharacterEntity entity = platoon_.Squads[i].Members[j];
					await entity.LoadAI("Character/AI/edit");
				}

				if (platoon_.Squads[i].Leader != null) {
					await platoon_.Squads[i].Leader.LoadAI("Character/AI/edit");
				}
			}
		}

		public async void ChangePlatoon(int index) {
			for (int i = 0, max = platoon_.Squads.Count; i < max; i++) {
				platoon_.DestroySquad(0);
			}
			PlatoonAssembly.I.Destroy(platoon_);
			await CreatePlayerCharacterEntity(index);
			// Entityの実行状態への移行
			assemblyManager_.Flush();
		}

		/// <summary>
		/// 表示している小隊をデータに保存する
		/// </summary>
		public void UpdatePlatoonData() {
			OrganizeSituationData.I.Update(platoon_);
		}
	}


}
