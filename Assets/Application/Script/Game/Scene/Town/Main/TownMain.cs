using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;
using System.Threading;

namespace Project.Game {
	/// <summary>
	/// メイン処理
	/// </summary>
	public partial class TownMain : MonoBehaviour, ITownFacility {
		public static TownMainResponse TransitionData;


//インスタンス管理
#region Assembly
		//Entity集合クラス
		AssemlyManager assemblyManager_ = new AssemlyManager(
			new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
				CharacterAssembly.CreateInstance,

				TownhallAssembly.CreateInstance,
				FactoryAssembly.CreateInstance,
				ResidenceAssembly.CreateInstance,
				StorageAssembly.CreateInstance,
				MarketAssembly.CreateInstance,
				ServiceAssembly.CreateInstance,
				LinkAssembly.CreateInstance,

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
		//フィールド
		private FieldEntity stageField_ = null;
		public FieldEntity stageField { get { return stageField_; } }
		//カメラ
		private CameraEntity mainCamera_ = null;
		public CameraEntity MainCamera { get { return mainCamera_; } }
		//ライト
		private LightEntity directionalLight_ = null;
		public LightEntity DirectionalLight { get { return directionalLight_; } }
		//キャラクター関連
		PlatoonEntity[] team_ = new PlatoonEntity[(int)Power.Max];
		public PlatoonEntity[] Team { get { return team_; } }
#endregion

//ユーザー制御系のインスタンス
#region UserControl
		//ユーザー操作割り振り管理
		private UserOperation haveUserOperation_;
		public UserOperation HaveUserOperation { get { return haveUserOperation_; } }
#endregion


//ゲームのステート管理
#region GameState
		/// <summary>
		/// ステート管理用
		/// </summary>
		public enum State {
			None,				// 初期無効状態
			Download,			// 初期化前にデータをサーバからロード
			Initialize,			// 初期化処理
			Main,				// ゲームメイン
			FacilityBuild,		// 建物仮設置から確定まで
			FacilityReLocate,	// 建物再配置から確定まで

			Max,
		}
		// ステート管理
		private StateMachine<TownMain> haveState_ = null;
		public StateMachine<TownMain> HaveState { get { return haveState_; } }
#endregion

		UniqueCounter counter_ = new UniqueCounter();
		//タウンホール
		Townhall townhall_;
		public Townhall CenterTownhall { get { return townhall_; } }
		//生産施設
		List<Factory> factories_ = new List<Factory>();
		public List<Factory> Factories { get { return factories_; } }
		//住居施設
		List<Residence> residences_ = new List<Residence>();
		public List<Residence> Residences { get { return residences_; } }
		//倉庫
		List<Storage> storages_ = new List<Storage>();
		public List<Storage> Storages { get { return storages_; } }
		//マーケット
		List<Market> markets_ = new List<Market>();
		public List<Market> Markets { get { return markets_; } }
		//サービス
		List<Service> services_ = new List<Service>();
		public List<Service> Services { get { return services_; } }

		//施設全体の情報
		Dictionary<int, Facility> allFacilities_ = new Dictionary<int, Facility>();
		public Dictionary<int, Facility> AllFacilities { get { return allFacilities_; } }


		ChainLinkMap chainMap_ = new ChainLinkMap();
		public ChainLinkMap ChainMap { get { return chainMap_; } }

		//本体のTransform
		protected Transform cacheTrans_;
		public Transform CacheTrans { get { return cacheTrans_; } }

		private TownGridMap gridMap_;
		public TownGridMap GridMap { get { return gridMap_; } }
		/// <summary>
		/// ステート変更
		/// </summary>
		public void ChangeState(State state) {
			haveState_.ChangeState(this, (int)state);
		}
		/// <summary>
		/// 状態の登録
		/// </summary>
		private void InitState(State state) {
			haveState_ = new StateMachine<TownMain>((int)State.Max);
			haveState_.Register<StateTownNone>(gameObject, (int)State.None);
			haveState_.Register<StateTownDownload>(gameObject, (int)State.Download);
			haveState_.Register<StateTownInitialize>(gameObject, (int)State.Initialize);
			haveState_.Register<StateTownMain>(gameObject, (int)State.Main);
			haveState_.Register<StateTownFacilityBuild>(gameObject, (int)State.FacilityBuild);
			haveState_.Register<StateTownFacilityReLocate>(gameObject, (int)State.FacilityReLocate);

			//初期StateセットだけだとEnterに入らないので一旦無効ステートを初期ステートにした後で初期ステートに変更
			haveState_.SetFirstState((int)State.None);

			ChangeState(state);
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		private void Awake() {
#if DEVELOP_BUILD
			EntryPoint.DirectBoot(EntryPoint.BootScene.Town, () => { TownMainCmd.CreateAsync(new TownMainRequest(), (response) => { TransitionData = (response as TownMainResponse); }); });
			TownDebugKeyControl.Create(this);
			TownDebugRoot.Add();
#endif
			AddressableChecker.CheckPoint("Town Start");
			MessageSystem.Create(new TownMessageSetting());

			cacheTrans_ = transform;
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);

			//ユーザー操作の排他制御（他の制御より前にインスタンス作る）
			haveUserOperation_ = new UserOperation((int)MessageGroup.TouchControl);

			assemblyManager_.Create(cacheTrans_);
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
			TownAlternativeMenu.Close();
			TownDebugRoot.Remove();
#endif

			System.GC.Collect();
			AddressableChecker.CheckPoint("Town End");
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
		private void LateUpdate() {
			//Entityの実行後処理
			assemblyManager_.LateExecute();

			gridMap_?.Execute(this);
		}

		/// <summary>
		/// バトルシーンの初期化
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
			// Entityの実行状態への移行
			assemblyManager_.Flush();

			mainCamera_.ChangeControlView(true);
			haveUserOperation_.SetDefault((int)OperationPriority.CameraViewing);

		}
#if DEVELOP_BUILD
		/// <summary>
		/// バトルシーンのデバッグ機能初期化
		/// </summary>
		public void InitializeDebug() {
			TownAlternativeMenu.Open(this);
			TownDebugSetting.Setting();
			GameObject obj = new GameObject("Debug");
		}
#endif

		/// <summary>
		/// フィールド生成
		/// </summary>
		public async Task CreateFieldEntity() {
			gridMap_ = new TownGridMap(GameConst.Town.FIELD_SIZE_WIDTH, GameConst.Town.FIELD_SIZE_DEPTH, transform);
			stageField_ = await FieldAssembly.I.CreateAsync(AddressableDefine.Address.TOWN_STAGE_SCENE, ResourcesPath.FIELD_SKY_MATERIAL);
			stageField_.InitSize(new Vector2(GameConst.Town.FIELD_SIZE_WIDTH, GameConst.Town.FIELD_SIZE_DEPTH));

		}
		/// <summary>
		/// カメラ生成
		/// </summary>
		public async Task CreateCameraEntity() {
			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.VIEW_CAMERA_PREFAB);
			mainCamera_.ControlSetting.Init(60f, 20f);
			mainCamera_.ControlSetting.SetLengthMinMax(20f, 100f, 10f);
			mainCamera_.ControlSetting.SetAzimthMinMax(float.MinValue, float.MaxValue);
			mainCamera_.LimitTargetView(-stageField_.Width * 0.5f, stageField_.Width * 0.5f, -stageField_.Depth * 0.5f, stageField_.Depth * 0.5f);

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
		/// 生産施設設置
		/// </summary>
		public async Task<Facility> CreateFactory(int baseId) {
			var mst = BaseDataManager.GetDictionary<int, MstFactoryData>()[baseId];
			Factory entity = await FactoryAssembly.I.CreateAsync(counter_.GetUniqueId(), mst);
			factories_.Add(entity);

			BuildFacility(entity);

			//準備が終わったので生成したEntityを書き出してゲーム内に登場させる
			FactoryAssembly.I.Flush();
			return entity;
		}
		/// <summary>
		/// 生産施設破棄
		/// </summary>
		public void DestroyFactory(Factory entity) {
			//チェーンを破棄
			entity.DestroyChain();

			//労働者の割り当てを解放
			PeopleData.I.ResetAssign(entity.Id);

			allFacilities_.Remove(entity.Id);
			factories_.Remove(entity);
			FactoryAssembly.I.Destroy(entity);

			haveUserOperation_.Reset();

		}

		/// <summary>
		/// マーケット設置
		/// </summary>
		public async Task<Facility> CreateMarket(int baseId) {
			var mst = BaseDataManager.GetDictionary<int, MstMarketData>()[baseId];
			Market entity = await MarketAssembly.I.CreateAsync(counter_.GetUniqueId(), mst);
			markets_.Add(entity);
			BuildFacility(entity);

			//準備が終わったので生成したEntityを書き出してゲーム内に登場させる
			MarketAssembly.I.Flush();
			return entity;
		}
		/// <summary>
		/// マーケット破棄
		/// </summary>
		public void DestroyMarket(Market entity) {
			//チェーンを破棄
			entity.DestroyChain();

			markets_.Remove(entity);
			MarketAssembly.I.Destroy(entity);

			haveUserOperation_.Reset();
		}


		/// <summary>
		/// 住居設置
		/// </summary>
		public async Task<Facility> CreateResidence(int baseId) {
			var mst = BaseDataManager.GetDictionary<int, MstResidenceData>()[baseId];
			Residence entity = await ResidenceAssembly.I.CreateAsync(counter_.GetUniqueId(), mst);
			residences_.Add(entity);
			BuildFacility(entity);

			//準備が終わったので生成したEntityを書き出してゲーム内に登場させる
			ResidenceAssembly.I.Flush();
			return entity;
		}

		/// <summary>
		/// 住居破棄
		/// </summary>
		public void DestroyResidence(Residence entity) {
			//チェーンを破棄
			entity.DestroyChain();
			//住人の割り当てを解放
			PeopleData.I.ResetStay(entity.Id);

			residences_.Remove(entity);
			ResidenceAssembly.I.Destroy(entity);

			haveUserOperation_.Reset();
		}

		/// <summary>
		/// 倉庫施設設置
		/// </summary>
		public async Task<Facility> CreateStorage(int baseId) {
			var mst = BaseDataManager.GetDictionary<int, MstStorageData>()[baseId];
			Storage entity = await StorageAssembly.I.CreateAsync(counter_.GetUniqueId(), mst);
			storages_.Add(entity);
			BuildFacility(entity);

			//準備が終わったので生成したEntityを書き出してゲーム内に登場させる
			StorageAssembly.I.Flush();
			return entity;
		}

		/// <summary>
		/// サービス施設破棄
		/// </summary>
		public void DestroyStorage(Storage entity) {
			//チェーンを破棄
			entity.DestroyChain();

			storages_.Remove(entity);
			StorageAssembly.I.Destroy(entity);

			haveUserOperation_.Reset();
		}

		/// <summary>
		/// サービス施設設置
		/// </summary>
		public async Task<Facility> CreateService(int baseId) {
			var mst = BaseDataManager.GetDictionary<int, MstServiceData>()[baseId];
			Service entity = await ServiceAssembly.I.CreateAsync(counter_.GetUniqueId(), mst);
			services_.Add(entity);
			BuildFacility(entity);

			//準備が終わったので生成したEntityを書き出してゲーム内に登場させる
			ServiceAssembly.I.Flush();
			return entity;
		}

		/// <summary>
		/// 倉庫施設破棄
		/// </summary>
		public void DestroyService(Service entity) {
			//チェーンを破棄
			entity.DestroyChain();

			services_.Remove(entity);
			ServiceAssembly.I.Destroy(entity);

			haveUserOperation_.Reset();
		}
		/// <summary>
		/// 建物を一括クリア
		/// </summary>
		public void Clear() {
			TownhallAssembly.I.Destroy(townhall_);
			//今ある状態を全破棄
			for (int i = 0, max = factories_.Count; i < max; i++) {
				FactoryAssembly.I.Destroy(factories_[i]);
			}
			for (int i = 0, max = residences_.Count; i < max; i++) {
				ResidenceAssembly.I.Destroy(residences_[i]);
			}
			for (int i = 0, max = storages_.Count; i < max; i++) {
				StorageAssembly.I.Destroy(storages_[i]);
			}
			for (int i = 0, max = markets_.Count; i < max; i++) {
				MarketAssembly.I.Destroy(markets_[i]);
			}
			for (int i = 0, max = services_.Count; i < max; i++) {
				ServiceAssembly.I.Destroy(services_[i]);
			}
			factories_.Clear();
			residences_.Clear();
			storages_.Clear();
			markets_.Clear();
			services_.Clear();
		}

		/// <summary>
		/// セーブされたデータをもとに町をレイアウトする（同期呼び出しバージョン）
		/// </summary>
		public async void LoadTown(TownData data) {
			await LoadTownAsync(data);
		}

		/// <summary>
		/// セーブされたデータをもとに町をレイアウトする
		/// </summary>
		public async Task LoadTownAsync(TownData data) {
			Clear();

			counter_ = new UniqueCounter(data.identifer);

			TownhallData townhall = data.townhall;
			townhall_ = await TownhallAssembly.I.CreateAsync(townhall);
			gridMap_.Fill(townhall_.GetFillTile());

			//生産施設
			for (int i = 0, max = data.factories.Count; i < max; i++) {
				FactoryData factory = data.factories[i];
				Factory entity = await FactoryAssembly.I.CreateAsync(factory);
				gridMap_.Fill(entity.GetFillTile());
				factories_.Add(entity);
				allFacilities_[entity.Id] = entity;
			}

			//マーケット
			for (int i = 0, max = data.markets.Count; i < max; i++) {
				MarketData market = data.markets[i];
				Market entity = await MarketAssembly.I.CreateAsync(market);
				gridMap_.Fill(entity.GetFillTile());
				markets_.Add(entity);
				allFacilities_[entity.Id] = entity;
			}

			//住居
			for (int i = 0, max = data.residences.Count; i < max; i++) {
				ResidenceData residence = data.residences[i];
				Residence entity = await ResidenceAssembly.I.CreateAsync(residence);
				gridMap_.Fill(entity.GetFillTile());
				residences_.Add(entity);
				allFacilities_[entity.Id] = entity;
			}
			//倉庫
			for (int i = 0, max = data.storages.Count; i < max; i++) {
				StorageData storage = data.storages[i];
				Storage entity = await StorageAssembly.I.CreateAsync(storage);
				gridMap_.Fill(entity.GetFillTile());
				storages_.Add(entity);
				allFacilities_[entity.Id] = entity;
			}
			//サービス
			for (int i = 0, max = data.services.Count; i < max; i++) {
				ServiceData service = data.services[i];
				Service entity = await ServiceAssembly.I.CreateAsync(service);
				gridMap_.Fill(entity.GetFillTile());
				services_.Add(entity);
				allFacilities_[entity.Id] = entity;
			}

			//Chainをつなぐ
			for (int i = 0, max = data.chainList.Count; i < max; i++) {
				ChainData chain = data.chainList[i];
				Facility.EnChain(allFacilities_[chain.senderId], allFacilities_[chain.recieverId], chain.valid);
			}
			//流通ポイントを計算
			CalculateLogistics();
		}
		/// <summary>
		/// 新規建物の配置を決める操作に移る
		/// </summary>
		private void BuildFacility(Facility entity) {
			//Idが重複してる可能性がある
			Debug.Assert(!allFacilities_.ContainsKey(entity.Id), "alread assigned facility id:" + entity.Id);
			allFacilities_[entity.Id] = entity;
			//チェーンを生成
			entity.CreateChain(this);
		}

		/// <summary>
		/// 流通ポイントを計算
		/// </summary>
		/// <remarks>
		/// 流通ポイントは建物の流通半径の合計で定義
		/// </remarks>
		public void CalculateLogistics()
		{
			float point = 0;
			foreach (var facility in allFacilities_.Values) {
				point += facility.HaveArea.Radius;
			}

			townhall_.HaveParam.Logistics = (int)point;
		}

		/// <summary>
		/// データ送信
		/// </summary>
		public TownData Send() {
			TownData data = new TownData();
			data.identifer = counter_.Id;

			data.townhall = townhall_.ToNetworkData();

			//生産施設
			data.factories = new List<FactoryData>();
			for (int i = 0, max = factories_.Count; i < max; i++) {
				data.factories.Add(factories_[i].ToNetworkData());
			}

			//マーケット
			data.markets = new List<MarketData>();
			for (int i = 0, max = markets_.Count; i < max; i++) {
				data.markets.Add(markets_[i].ToNetworkData());
			}

			//住居
			data.residences = new List<ResidenceData>();
			for (int i = 0, max = residences_.Count; i < max; i++) {
				data.residences.Add(residences_[i].ToNetworkData());
			}
			//倉庫
			data.storages = new List<StorageData>();
			for (int i = 0, max = storages_.Count; i < max; i++) {
				data.storages.Add(storages_[i].ToNetworkData());
			}
			//サービス
			data.services = new List<ServiceData>();
			for (int i = 0, max = services_.Count; i < max; i++) {
				data.services.Add(services_[i].ToNetworkData());
			}


			return data;
		}

	}
}
