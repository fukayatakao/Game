using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;
using Project.Http.Mst;

namespace Project.Game {
    /// <summary>
    /// キャラクター
    /// </summary>
	public class CharacterEntity : Entity {
		//管理Id
		public int Id { get; private set; }
		//ユニットデータ
		public MstUnitData HaveUnitMaster { get; private set; }
		//大将フラグ
		public bool IsLeader { get { return havePersonal_.IsLeader(); } }
		//所属チーム
		public PlatoonEntity Platoon { get { return Squad.Platoon; } }
		//所属分隊
		public SquadEntity Squad { get; private set; }

		public bool IsAlive { get { return IsActive() && haveUnitParam_.Fight.IsAlive(); } }

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		//@note rederer系は描画形式で処理が大きく変わりそうなので外に露出させずラッパー関数を通してアクセスする
		//マテリアル処理クラス
		private RenderMaterial haveRenderMaterial_;

		private CharacterModel haveModel_;
		public CharacterModel HaveModel { get { return haveModel_; } }

		public CharacterAnimation HaveAnimation { get{ return haveModel_.HaveAnimation; } }
		public CharacterOutline HaveOutline { get { return haveModel_.HaveOutline; } }
		public CharacterNodeCache HaveCacheNode { get { return haveModel_.HaveCacheNode; } }

        //ユーザーの操作による制御
        UserControl haveUserControl_;
        System.Action control_ = ()=>{ };

        //キャラクターのユニットパラメータ
        private CharacterUnitParam haveUnitParam_;
        public CharacterUnitParam HaveUnitParam{ get{ return haveUnitParam_; } }

        //キャラクターの個人パラメータ
        private CharacterPersonal havePersonal_;
        public CharacterPersonal HavePersonal{ get{ return havePersonal_; } }

         //AI用の記憶領域
        CharacterBlackboard haveBlackboard_;
        public CharacterBlackboard HaveBlackboard{ get{ return haveBlackboard_; } }

		//キャラクターのAI制御
        private CharacterThink haveThink_;
        public CharacterThink HaveThink { get { return haveThink_; } }

		//キャラクターの身体状態
        private CharacterState haveState_;
        public CharacterState HaveState { get { return haveState_; } }

		//キャラクターの選択サークル
		private CharacterCircle haveCircle_;
		public CharacterCircle HaveCircle { get { return haveCircle_; } }

		//キャラに付随するHP/LPバーなどのUI
		private CharacterFloatGauge haveFloatGauge_;
        public CharacterFloatGauge HaveFloatGauge { get { return haveFloatGauge_; } }
		//キャラの属性アイコンUI
		private CharacterPhaseIcon havePhaseIcon_;
		public CharacterPhaseIcon HavePhaseIcon { get { return havePhaseIcon_; } }
		//キャラクター名テキスト
		private CharacterNameText haveNameText_;
		public CharacterNameText HaveNameText { get { return haveNameText_; } }

		//@note 設定関数を隠蔽するためラップ関数でアクセスする
		//座標関連の情報
		private CharacterPosition havePosition_;
		public CharacterPosition HavePosition { get { return havePosition_; } }

		//そのフレームでの移動ベクトル
		private Vector3 moveVector_;
        public Vector3 MoveVector { get { return moveVector_; } }

		//キャラコリジョン
		private BodyCollision haveCollision_;
        public BodyCollision HaveCollision { get { return haveCollision_; } }

		private CharacterAction haveAction_;
		public CharacterAction HaveAction { get { return haveAction_; } }

		private CharacterSkill haveSkill_;
		public CharacterSkill HaveSkill { get { return haveSkill_; } }

		//並列タスク
		private CoroutineTaskParallel parallelTask_ = new CoroutineTaskParallel();
		public CoroutineTaskParallel ParallelTask{get{ return parallelTask_; } }

		private CoroutineTaskList taskList_ = new CoroutineTaskList();
		public CoroutineTaskList TaskList{get{ return taskList_; } }

		public bool ExecuteEnable;
		//GameObject名が同じにならないように末尾に番号つける
        static Dictionary<string, int> tailCounter_ = new Dictionary<string, int>();
#if DEVELOP_BUILD
	    private CharacterDebug haveDebug_;
	    public CharacterDebug HaveDebug { get { return haveDebug_; } }
#endif

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public override async Task<GameObject> CreateAsync(string name) {
			string[] n = name.Split('/');
			string objectName = n.Length != 0 ? n[n.Length - 1] : name;
			GameObject obj = new GameObject(objectName);
			haveModel_ = MonoPretender.Create<CharacterModel>(obj);
			await haveModel_.LoadAsync(obj.transform, name);
			//コンポーネントからEntityを手繰る用
			haveModel_.Model.AddComponent<CharacterPortal>().Init(this);
			haveModel_.Init();
			CreateImpl(obj);
			return obj;
		}

#if UNITY_EDITOR
        /// <summary>
        /// インスタンス生成時処理(エディタ用)
        /// </summary>
        public override GameObject Create(GameObject resObject) {
			GameObject obj = new GameObject("character");
			haveModel_ = MonoPretender.Create<CharacterModel>(obj);
			haveModel_.Create(obj.transform, resObject);
			haveModel_.Model.AddComponent<CharacterPortal>().Init(this);
			haveModel_.Init();
			CreateImpl(obj);
			return obj;
        }
#endif
        /// <summary>
        /// 生成の本番・エディタ共通処理
        /// </summary>
        private void CreateImpl(GameObject main) {
            if (!tailCounter_.ContainsKey(main.name)) {
                tailCounter_[main.name] = 0;
            } else {
                tailCounter_[main.name]++;
            }
			main.name = main.name + "_" + tailCounter_[main.name];


            //メッセージ受容体作成
            receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent, MessageGroup.DebugEvent);

			CreateBehaviour(main);
		}

		/// <summary>
		/// Createの中で行う設定処理
		/// </summary>
		private void CreateBehaviour(GameObject obj) {
			haveRenderMaterial_ = MonoPretender.Create<RenderMaterial>(obj);
			haveState_ = MonoPretender.Create<CharacterState>(obj);
            haveBlackboard_ = MonoPretender.Create<CharacterBlackboard>(obj);
			havePosition_ = MonoPretender.Create<CharacterPosition>(obj);
			haveThink_ = MonoPretender.Create<CharacterThink>(obj);
            haveCollision_ = MonoPretender.Create<BodyCollision>(obj);
			haveAction_ = MonoPretender.Create<CharacterAction>(obj);
			haveSkill_ = MonoPretender.Create<CharacterSkill>(obj);;
			haveUnitParam_ = MonoPretender.Create<CharacterUnitParam>(obj);
			havePersonal_ =  MonoPretender.Create<CharacterPersonal>(obj);
			haveCircle_ = MonoPretender.Create<CharacterCircle>(obj);

			haveFloatGauge_ = MonoPretender.Create<CharacterFloatGauge>(obj);
			havePhaseIcon_ = MonoPretender.Create<CharacterPhaseIcon>(obj);;
			haveNameText_ = MonoPretender.Create<CharacterNameText>(obj);;
#if DEVELOP_BUILD
			haveDebug_ = MonoPretender.Create<CharacterDebug>(obj);
#endif
			ExecuteEnable = true;
		}
		public async Task LoadAnimation(string animeName) {
			await haveModel_.HaveAnimation.LoadAsync(animeName);
		}

		public async Task LoadAction(string actionName) {
			await haveAction_.LoadAsync(actionName);
		}

		public async Task LoadAI(string aiName) {
			await haveThink_.LoadAsync(aiName);
		}

		public async Task LoadCircle() {
			await haveCircle_.LoadAsync();
			haveCircle_.Setup(cacheTrans_, haveCollision_.Radius, Color.red);
		}
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(MstUnitData mstUnit, CharacterData data, Transform parent) {
			//キャラクターパラメータ
			InitUnit(mstUnit);
			InitPersonal(data);
			SetParent(parent);
			haveModel_.HaveAnimation.InitPlay(BattleMotion.Idle);
			ChangeState(CharacterState.State.Idle);

			float hp = haveUnitParam_.Fight.Hp;
			float lp = haveUnitParam_.Fight.Lp;
			haveFloatGauge_.SetParam(hp, lp);
#if DEVELOP_BUILD
			SetupDebug(gameObject_);
#endif
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(MstUnitData mstUnit, LeaderData leaderData, Transform parent) {
			//キャラクターパラメータ
			InitUnit(mstUnit);
			InitPersonal(leaderData);
			SetParent(parent);
			haveModel_.HaveAnimation.InitPlay(BattleMotion.Idle);
			ChangeState(CharacterState.State.Idle);

			float hp = haveUnitParam_.Fight.Hp;
			float lp = haveUnitParam_.Fight.Lp;
			haveFloatGauge_.SetParam(hp, lp);
#if DEVELOP_BUILD
			SetupDebug(gameObject_);
#endif
		}

		/// <summary>
		/// UI設定初期化
		/// </summary>
		public void InitUI(CameraEntity cameraEntity, Transform canvas) {
			haveFloatGauge_.Init(cameraEntity.Camera, canvas);
			havePhaseIcon_.Init(cameraEntity.Camera, canvas);
			haveNameText_.Init(cameraEntity.Camera, canvas);
#if DEVELOP_BUILD
			SetupDebugUI(canvas.gameObject, cameraEntity.Camera);
#endif
			havePhaseIcon_.SetPhaseSprite((PHASE)havePersonal_.Phase);
		}

		/// <summary>
		/// 肉体を入替
		/// </summary>
		public async void ChangeModel(Mst.MstUnitData data, System.Action callback) {
			HaveUnitMaster = data;
			var asset = await AddressableAssist.LoadAssetAsync(data.ModelName);
			haveModel_.Change(this, asset);
			await haveModel_.HaveAnimation.LoadAsync(data.AnimationLabel);

			callback();
		}
		/// <summary>
		/// 魂を入替
		/// </summary>
		public CharacterData ChangePersonal(CharacterData data) {
			CharacterData old = ConvertCharacterData();
			InitPersonal(data);
			return old;
		}

		/// <summary>
		/// ユニット情報をセット
		/// </summary>
		private void InitUnit(MstUnitData mst) {
			//キャラクターパラメータ
			HaveUnitMaster = mst;
			haveUnitParam_.Physical.Init(mst);
			haveUnitParam_.Fight.Init(mst);
			haveBlackboard_.AttackBoard.Init(mst);
		}
	    /// <summary>
	    /// 一般キャラとしてパーソナルデータを初期化
	    /// </summary>
		private void InitPersonal(CharacterData data) {
			//キャラクターパラメータ
			if (data == null) {
				Id = -1;
				havePersonal_.Init(CharacterUtil.GetEmptyCharacterData());
			} else {
				Id = data.id;
				havePersonal_.Init(data);
			}
		}

	    /// <summary>
	    /// リーダーとしてパーソナルデータを初期化
	    /// </summary>
		private void InitPersonal(LeaderData data) {
			//キャラクターパラメータ
			Id = data.id;
			havePersonal_.Init(data);

			//リーダースキル設定
			haveSkill_.Init(data);
		}

		/// <summary>
		/// 編成シーン用設定
		/// </summary>
		public void SetupEdit() {
			//遅延なし
			haveState_.StateDelay.Disable();
			//移動遅いとストレスなので速度を上げる
			haveUnitParam_.Physical.Acceleration *= 3f;
			haveUnitParam_.Physical.MaxSpeed *= 3f;
		}

		/// <summary>
		/// 存在チェック
		/// </summary>
		public override bool IsExist() {
			if (IsActive())
				return true;
			if (IsAlive)
				return true;
			return false;
		}

#if DEVELOP_BUILD
		/// <summary>
		/// デバッグ表示の設定
		/// </summary>
		private void SetupDebug(GameObject obj) {
			GameObject debugObj = UnityUtil.InstantiateChild(obj.transform, "Debug");
			//攻撃パターン分回す
			haveDebug_.AttackArea = new DebugAttackArea[(int)ACTION_PATTERN.MAX];
			for (int i = 0; i < (int)ACTION_PATTERN.MAX; i++) {
				DebugAttackArea attackArea = debugObj.AddComponent<DebugAttackArea>();
				attackArea.Init(this, (ACTION_PATTERN)i);
				attackArea.SetParent(debugObj.transform);
				haveDebug_.AttackArea[i] = attackArea;
			}

			DebugCollisionDraw debugCollDraw = debugObj.AddComponent<DebugCollisionDraw>();
			debugCollDraw.Init(this, obj.transform);
			debugCollDraw.SetParent(debugObj.transform);
			haveDebug_.CollisionDraw = debugCollDraw;

			DebugTargetDraw debugTarget = debugObj.AddComponent<DebugTargetDraw>();
			debugTarget.Init(this, obj.transform);
			haveDebug_.TargetDraw = debugTarget;

			DebugSearchArea searchArea = debugObj.AddComponent<DebugSearchArea>();
			searchArea.Init(this, obj);		//
			haveDebug_.SearchArea = searchArea;
		}
		GameObject debugObj;
		/// <summary>
		/// デバッグ表示の設定
		/// </summary>
		private void SetupDebugUI(GameObject canvas, Camera camera) {
			debugObj = UnityUtil.InstantiateChild(canvas.transform, "DebugUI");

			DebugAIState debugHeadText_ = debugObj.AddComponent<DebugAIState>();
			debugHeadText_.Init(this, camera);
			debugHeadText_.SetParent(debugObj.transform);
		}

		private void CleanupDebugUI() {
			//debugObjはcanvasについているためキャラが消えても消えないので明示的に破棄
			GameObject.Destroy(debugObj);
		}

#endif

		/// <summary>
		/// キャラクターの配置状態をセット
		/// </summary>
		public void InitLocation(Vector3 position, float sign) {
			havePosition_.DeployPosition = position;
			SetPosition(position);
            SetRotation(MathUtil.LookAtY(Vector3.forward * sign));
		}
		/// <summary>
		/// キャラクターの所属状態をセット
		/// </summary>
		public void InitAttitude(SquadEntity squad) {
			Squad = squad;
			//haveUnitParam_.Party.Init(squad, boss);
			haveModel_.HaveOutline.SetTeamColor(squad.Platoon.Index);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
#if UNITY_EDITOR
			CleanupDebugUI();
#endif
			MonoPretender.Destroy(haveModel_);

			MonoPretender.Destroy(haveRenderMaterial_);
			MonoPretender.Destroy(haveState_);
			MonoPretender.Destroy(haveBlackboard_);
			MonoPretender.Destroy(havePosition_);
			MonoPretender.Destroy(haveThink_);
			MonoPretender.Destroy(haveCollision_);
			MonoPretender.Destroy(haveAction_);
			MonoPretender.Destroy(haveSkill_);
			MonoPretender.Destroy(haveUnitParam_);
			MonoPretender.Destroy(havePersonal_);
			MonoPretender.Destroy(haveCircle_);

			MonoPretender.Destroy(haveFloatGauge_);
			MonoPretender.Destroy(havePhaseIcon_);
			MonoPretender.Destroy(haveNameText_);
#if DEVELOP_BUILD
			MonoPretender.Destroy(haveDebug_);
#endif
			MessageSystem.DestroyReceptor(receptor_);
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public override void Execute() {
			//演出で実行処理を止めている場合
			if (!ExecuteEnable)
				return;
			//実行可能な状態にないなら何もしない
			if (!IsActive())
				return;

			//タスク実行
			taskList_.Execute();
			parallelTask_.Execute();

			haveUnitParam_.Execute(this);
			haveFloatGauge_.Execute(this);
			haveNameText_.Execute(this);
			havePhaseIcon_.Execute(this);
			havePosition_.Execute(this);
			haveAction_.Execute(this);

            control_();
            haveState_.Execute(this);
			haveBlackboard_.Execute(this);

			haveThink_.Execute(this);

		}

		/// <summary>
		/// Playable更新
		/// </summary>
		public override void Evaluate() {
			//演出で実行処理を止めている場合
			if (!ExecuteEnable)
				return;
			haveModel_.HaveAnimation.LateExecute();
		}

		/// <summary>
		/// 実行後処理
		/// </summary>
		public override void LateExecute() {
			//演出で実行処理を止めている場合
			if (!ExecuteEnable)
				return;
			haveState_.LateExecute(this);

			//AddForceだとFixedUpdateに置かないと正しく動かない。
			//FixedUpdateだと描画とタイミングずれるので引きずられてほかの機能もFixedUpdateで動かすことになりそうなのでMovePositionで妥協
			//坂を下りたりするときにちょっと浮くので接地中フラグとかで強制的に地面に置くようにする
			//rigid_.MovePosition(rigid_.position + moveVector_);
			//rigid_.velocity = Vector3.zero;
			//agent_.Move(moveVector_ * Time.deltaTime);

			//進行ベクトルと前フレームの押し戻しベクトルの射影を取って押し戻された方向への移動ベクトルを相殺してめり込む移動を抑える
			if (Vector3.Dot(moveVector_, haveCollision_.Correct) > 0f){
				Vector3 n = haveCollision_.Correct.normalized;
				float dot = Vector3.Dot(moveVector_, n);
				moveVector_ = moveVector_ - n * dot;
			}
			SetPosition(GetPosition() + moveVector_);

            moveVector_ = Vector3.zero;
		}

		/// <summary>
		/// 物理計算実行処理
		/// </summary>
		public override void FixedExecute() {
            if (!IsActive())
                return;
            //rigid_.AddForce(cacheTrans_.right * 80f);
        }

		/// <summary>
		/// 接地計算
		/// </summary>
		public void Grounding() {
			Vector3 pos = GetPosition();
			//10m上げて下にray(100m)を飛ばして接地座標を計算
			pos.y += 10f;
			bool result = Physics.Raycast(pos, Vector3.down, out var hit, 100f, 1 << (int)UnityLayer.Layer.Field);
			if (result) {
				pos.y = hit.point.y;
				SetPosition(pos);
			}
		}

		/// <summary>
		/// スキル実行
		/// </summary>
		public void PlaySkill(string skill) {
			//SP発動に必要な量がなかったら失敗
			if (!Platoon.HaveSpecialPoint.UseSkill())
				return;

			haveState_.GetState<StateCharacterSkill>(CharacterState.State.Skill).SetUseSkill(skill);
			ChangeState(CharacterState.State.Skill);
		}

		/// <summary>
		/// ユーザー操作に切り替え
		/// </summary>
		public void ChangeUserControl(CameraEntity cam) {
			haveUserControl_ = new UserControl(this);
			haveUserControl_.SetCameraEntity(cam);
            control_ = haveUserControl_.Execute;
        }
        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        /// <remarks>
        /// 操作の強弱による速度変化で移動する
        /// </remarks>
        public void ControllerMove(Quaternion direction, float speed = 1f) {
            moveVector_ += direction * new Vector3(0f, 0f, speed);
            cacheTrans_.localRotation = direction;
        }
        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        public void Move(Quaternion direction, float speed = 1f) {
            moveVector_ += direction * new Vector3(0f, 0f, speed);
        }
        /// <summary>
        /// 移動量の補正
        /// </summary>
        public void CorrectMove(float z) {
            //反対方向に移動しようとしたら移動量を0に丸める
            if(moveVector_.z * z <= 0f) {
                moveVector_.z = 0f;
            }else{
                moveVector_.z = z;
            }
        }

		/// <summary>
		/// 死亡した瞬間の処理
		/// </summary>
		public void Dead() {
			Defeat();
            haveState_.ChangeState(this, CharacterState.State.Dead);
		}

		/// <summary>
		/// 逃亡した瞬間の処理
		/// </summary>
		public void Escape() {
			Defeat();
			haveState_.ChangeState(this, CharacterState.State.Escape);
		}

		/// <summary>
		/// ゲームから退場するときの処理
		/// </summary>
		private void Defeat() {
			//UI非表示
			haveFloatGauge_.Hide();
			haveNameText_.Hide();
			havePhaseIcon_.Hide();

			//死亡したので管理からはずす
			Squad.Defeat(this);
			BattleMessage.DefeatUnit.Broadcast(this);
			haveThink_.Enable = false;
		}

		/// <summary>
		/// キャラの場所変更(編成で使用
		/// </summary>
		public void OrderRelocation(CharacterEntity target) {
			havePosition_.DeployPosition = target.GetPosition();
		}
		/// <summary>
		/// キャラの場所変更(編成で使用
		/// </summary>
		public void OrderGroundTarget(Vector3 target) {
			havePosition_.DeployPosition = target;
		}
		/// <summary>
		/// 敵キャラを追跡
		/// </summary>
		public void OrderChase(CharacterEntity target) {
			haveBlackboard_.EnemyBoard.SetLockonEnemy(target);
			haveThink_.ChangeState(this, CharacterThink.State.Chase);
		}
		/// <summary>
		/// 対象キャラが攻撃範囲の中にいるか
		/// </summary>
		public bool IsInSearchRangeEnemy() {
			return IsInRange(haveBlackboard_.EnemyBoard.TargetEnemy, haveUnitParam_.Physical.SearchRange);
		}

		/// <summary>
		/// 範囲内に対象がいるか
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool IsInRange(CharacterEntity target, float range) {
			if (target == null)
				return false;
			float distanceSq = (target.GetPosition() - GetPosition()).sqrMagnitude;
			float r = range + target.haveCollision_.Radius;
			return (distanceSq <= r * r);
		}

		/// <summary>
		/// ステージの矩形範囲に収める
		/// </summary>
		public void ClampDeployPos(Rect rect) {
			havePosition_.DeployPosition = CharacterPosition.ClampPos(havePosition_.DeployPosition, haveCollision_.Radius, rect);
		}
		/// <summary>
		/// ステージの矩形範囲に収める
		/// </summary>
		public void ClampCharacterPos(Rect rect) {
			SetPosition(CharacterPosition.ClampPos(GetPosition(), haveCollision_.Radius, rect));
		}
		/// <summary>
		/// 展開位置をターゲット位置に指定する
		/// </summary>
		public void TargetDeploy() {
			havePosition_.status = CharacterPosition.Status.Deploy;
		}

		public void SetDeployPos(Vector3 pos) {
			havePosition_.DeployPosition = pos;
		}

		/// <summary>
		/// 死亡キャラの後始末
		/// </summary>
		public void Clear() {
			SetActive(false);
		}
		public void ChangeState(CharacterState.State state, bool force = false) {
			HaveState.ChangeState(this, state, force);
		}
		public void ChangeAIState(CharacterThink.State state) {
			HaveThink.ChangeState(this, state);
		}

		/// <summary>
		/// キャラ選択したときの表示をする
		/// </summary>
		public void SelectCharacter() {
			haveCircle_.SetVisible(true);
			haveCircle_.SetColor(Color.green);
		}
		/// <summary>
		/// キャラ選択をはずしたときの処理
		/// </summary>
		public void UnSelectCharacter() {
			haveCircle_.SetVisible(false);
		}
		/// <summary>
		/// キャラ選択したときの表示をする
		/// </summary>
		public void TargetCharacter() {
			haveCircle_.SetVisible(true);
			haveCircle_.SetColor(Color.red);
		}
		/// <summary>
		/// キャラ選択をはずしたときの処理
		/// </summary>
		public void UnTargetCharacter() {
			haveCircle_.SetVisible(false);
		}


		/// <summary>
		/// キャラクターデータに戻す
		/// </summary>
		public CharacterData ConvertCharacterData() {
			CharacterData data = new CharacterData();
			data.id = Id;
			data.name = havePersonal_.CharaName;
			data.species = havePersonal_.Species;
			data.generate = havePersonal_.Generate;
			data.portrait = havePersonal_.Portrait;
			data.ability = havePersonal_.Ability;
			data.phase = (int)havePersonal_.Phase;
			return data;
		}
		/// <summary>
		/// リーダーデータに戻す
		/// </summary>
		public LeaderData ConvertLeaderData() {
			LeaderData data = new LeaderData();
			data.id = Id;
			data.leaderMasterId = havePersonal_.LeaderId;
			return data;
		}

		// ----------------------------------------------------------------------------------------------------
		// マテリアル関連処理のラッパー関数群
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// シェーダ設定
		/// </summary>
		/*public virtual void SetShader(ShaderCache.ShaderType shaderType = ShaderCache.ShaderType.SHADER_TYPE_DEFAULT) {
            haveRenderMaterial_.SetShader(shaderType);
        }*/

		/// <summary>
		/// アルファを設定
		/// </summary>
		public void SetAlpha(float alpha) {
            haveRenderMaterial_.SetAlpha(alpha);
        }

        /// <summary>
        /// 表示設定
        /// </summary>
        public void SetHide(bool hide) {
            haveRenderMaterial_.SetActive(!hide);
        }
        /*// カラーの初期化
        public void InitColor() { haveRenderMaterial_.InitColor(); }
        // カラーをまとめて設定
        public void SetMaterialColor(Color multiCol, Color addCol) { haveRenderMaterial_.SetMaterialColor(multiCol, addCol); }
        // 乗算カラーを設定
        public void SetMultiColor(Color color) { haveRenderMaterial_.SetMultiColor(color); }
        // 加算カラーを設定
        public void SetAddColor(Color color) { haveRenderMaterial_.SetAddColor(color); }
        // 乗算カラー変化開始
        public void StartChangeMultiColor(Color color, float changeTime) { haveRenderMaterial_.StartChangeMultiColor(color, changeTime); }
        // 乗算カラー変化停止
        public void StopChangeMultiColor() { haveRenderMaterial_.StopChangeMultiColor(); }
        // 加算カラー変化開始
        public void StartChangeAddColor(Color color, float changeTime) { haveRenderMaterial_.StartChangeAddColor(color, changeTime); }
        // 加算カラー変化停止
        public void StopChangeAddColor() { haveRenderMaterial_.StopChangeAddColor(); }
        // ヒットフラッシュ
        //public void HitFlash() { haveRenderMaterial_.HitFlash(); }
        // 画面外に出た時にアニメーションを止めるかどうか
        public void SetRendererUpdateWhenOffscreen(bool isEnable) { haveRenderMaterial_.SetRendererUpdateWhenOffscreen(isEnable); }*/

    }
}
