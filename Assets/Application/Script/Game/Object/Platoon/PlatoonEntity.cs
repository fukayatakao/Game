using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using System.Threading.Tasks;
using Project.Network;

namespace Project.Game {
    /// <summary>
    /// 小隊
    /// </summary>
    /// <remarks>
    /// 複数の分隊の制御クラス
    /// </remarks>
    public class PlatoonEntity : Entity {
	    //初期配置場所のオフセット
	    private const float LOCATION_OFFSET = 1.5f;
		//管理Id
		public int Id { get; private set; }
		public string PlatoonName;
		public int Experience;
		//所属
		public Power Index;
		public float TowardSign;
		//有効フラグ
		public bool Enable = false;
		//分隊が1列以上残っている場合は生存
		public bool IsAlive { get { return IsActive() && squads_.Count > 0; } }


		//所属分隊
		List<SquadEntity> squads_ = new List<SquadEntity>();
        public IReadOnlyList<SquadEntity> Squads{ get{ return squads_; } }

		//殲滅された分隊
		List<SquadEntity> defeatSquads_ = new List<SquadEntity>();
		public IReadOnlyList<SquadEntity> DefeatSquads { get { return defeatSquads_; } }

		//記憶領域
		PlatoonBlackboard haveBlackboard_;
		public PlatoonBlackboard HaveBlackboard { get { return haveBlackboard_; } }
		//AI制御
		private PlatoonThink haveThink_;
		public PlatoonThink HaveThink { get { return haveThink_; } }
		//展開位置計算
		private PlatoonDeploy haveDeploy_;

		//AI制御
		private PlatoonLimit haveLimit_;
		public PlatoonLimit HaveLimit { get { return haveLimit_; } }

		//SP値管理
		private PlatoonSpecialPoint haveSpecialPoint_;
		public PlatoonSpecialPoint HaveSpecialPoint { get { return haveSpecialPoint_; } }

		//列のデフォルト展開位置。各列はこの位置より後ろに展開ラインを引けない。
		private List<float> backRank_;



		//メッセージ受け取り用
		MessageSystem.Receptor receptor_;
		/// <summary>
		/// インスタンス生成
		/// </summary>
		public override async Task<GameObject> CreateAsync(string resName) {
			GameObject obj = new GameObject(resName);
			haveBlackboard_ = MonoPretender.Create<PlatoonBlackboard>(obj);
			haveDeploy_ = MonoPretender.Create<PlatoonDeploy>(obj);
			//AI用インスタンス生成
			haveThink_ = MonoPretender.Create<PlatoonThink>(obj);
			haveLimit_ = MonoPretender.Create<PlatoonLimit>(obj);
			haveSpecialPoint_ = MonoPretender.Create<PlatoonSpecialPoint>(obj);
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent, MessageGroup.DebugEvent);
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);

			return obj;
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(haveBlackboard_);
			MonoPretender.Destroy(haveDeploy_);
			MonoPretender.Destroy(haveThink_);
			MonoPretender.Destroy(haveLimit_);
			MonoPretender.Destroy(haveSpecialPoint_);
			MessageSystem.DestroyReceptor(receptor_);
		}


		/// <summary>
		/// 小隊情報をセット
		/// </summary>
		public void SetData(Network.PlatoonData data) {
			Id = data.id;
			PlatoonName = data.name;
			Experience = data.experience;
		}

		/// <summary>
		/// パラメータをセット
		/// </summary>
		public void InitParam(Power ensign, float width, float centerDepth, float territoryDepth, bool isDistortion=true) {
			Index = ensign;
			TowardSign = GameConst.TOWARD_SIGN[(int)ensign];

			haveBlackboard_.Init(this);
			haveSpecialPoint_.Init();

			//初期位置を設定
			for (int i = 0, max = squads_.Count; i < max; i++) {
				//展開ラインの設定
				squads_[i].InitLine(width, TowardSign);
				//所属情報を設定
				squads_[i].InitAttitude(this, (Abreast)i);
				//熟練度ボーナスを設定
				squads_[i].InitParam(Experience);
			}

			InitLocation(centerDepth, territoryDepth, isDistortion);
		}
		/// <summary>
		/// 座標配置
		/// </summary>
		public void InitLocation(float centerDepth, float territoryDepth, bool isDistortion=true) {
			//分隊数で等間隔に配置
			int div = squads_.Count;
			float d = territoryDepth / div;
			float s = centerDepth * 0.5f + LOCATION_OFFSET;
			backRank_ = new List<float>();
			for(int i = 0; i < div; i++) {
				backRank_.Add(s * -TowardSign);
				s += d;
			}

			CalcBackRankDepth();
			//初期位置を設定
			for (int i = 0, max = squads_.Count; i < max; i++) {
				//所属キャラを生成
				squads_[i].InitLocation(backRank_[i], TowardSign, isDistortion);
			}
		}

		/// <summary>
		/// UI設定初期化
		/// </summary>
		public void InitUI(CameraEntity cameraEntity, Transform canvas) {
			//初期位置を設定
			for (int i = 0, max = squads_.Count; i < max; i++) {
				//所属キャラを生成
				squads_[i].InitUI(cameraEntity, canvas);
			}
		}

		/// <summary>
		/// 各グループの展開位置の限界を設定
		/// </summary>
		/// <remarks>
		/// 後方の端に固まった状態が出来ないように列ごとに限界を設定する
		/// </remarks>
		private void CalcBackRankDepth() {
			//初期位置を設定
			for (int i = 0, max = squads_.Count; i < max; i++) {
				//ステージの端から起算して一番後ろの限界ラインを設定
				squads_[i].HaveDeployLine.SetLimit(backRank_[i]);
			}
		}

		/// <summary>
		/// 分隊を生成
		/// </summary>
		public async Task<SquadEntity> CreateSquadAsync(SquadData data, bool isImmediate) {
			//列を設定
			SquadEntity squad = await SquadAssembly.I.CreateAsync("Squad", data, isImmediate);
			squad.SetParent(cacheTrans_);
			squads_.Add(squad);
			return squad;
		}
		/// <summary>
		/// 分隊を生成
		/// </summary>
		public async void CreateSquadAsync(int abreast, int unitId, LeaderData leader, bool isImmediate, System.Action<SquadEntity> callback) {
			//列を設定
			SquadData data = new SquadData();
			data.unitId = unitId;
			data.members = new List<CharacterData>();
			for (int i = 0, max = MstUnitData.GetData(unitId).Fill; i < max; i++) {
				data.members.Add(null);
			}
			data.leader = leader;
			var squad = await CreateSquadAsync(data, isImmediate);

			//所属情報を設定
			squad.InitAttitude(this, (Abreast)abreast);


			for (int i = 0, max = squad.Members.Count; i < max; i++) {
				squad.Members[i].HaveThink.Enable = true;
				squad.Members[i].InitAttitude(squad);
				squad.Members[i].SetupEdit();
			}
			for (int j = 0, max2 = squad.Members.Count; j < max2; j++) {
				CharacterEntity entity = squad.Members[j];
				await entity.LoadAI("Character/AI/edit");
			}
			if (squad.Leader != null) {
				squad.Leader.HaveThink.Enable = false;
				squad.Leader.InitAttitude(squad);
			}


			callback(squad);
		}
		/// <summary>
		/// 列ユニット変更
		/// </summary>
		public async void ChangeUnitAsync(int abreast, int unitId, bool isImmediate, System.Action callback) {
			SquadEntity squad = squads_[abreast];
			squad.DestroyAllMembers();

			var mst = BaseDataManager.GetDictionary<int, MstUnitData>()[unitId];

			//所属キャラを生成
			await squad.CreateMember(mst, new List<CharacterData>(new CharacterData[mst.Fill]), isImmediate);
			for (int i = 0, max = squad.Members.Count; i < max; i++) {
				squad.Members[i].HaveThink.Enable = true;
				squad.Members[i].InitAttitude(squad);
				squad.Members[i].SetupEdit();
			}
			for (int j = 0, max2 = squad.Members.Count; j < max2; j++) {
				CharacterEntity entity = squad.Members[j];
				await entity.LoadAI("Character/AI/edit");
			}

			callback();
		}

		/// <summary>
		/// 列リーダー変更
		/// </summary>
		public async void ChangeLeaderAsync(int abreast, LeaderData leaderData, bool isImmediate, System.Action callback) {
			SquadEntity squad = squads_[abreast];
			squad.DestroyLeader();
			await squad.CreateLeader(leaderData, isImmediate);
			squad.Leader.HaveThink.Enable = false;
			squad.Leader.InitAttitude(squad);

			callback();
		}

		/// <summary>
		/// 分隊を削除
		/// </summary>
		public void DestroySquad(int abreast) {
			SquadEntity squad = squads_[abreast];
			squad.DestroyLeader();
			squad.DestroyAllMembers();
			squads_.RemoveAt(abreast);

			SquadAssembly.I.Destroy(squad);
		}

		/// <summary>
		/// AIファイルをロード
		/// </summary>
		public async Task LoadAI(string aiName) {
			await haveThink_.LoadAsync(aiName);
		}
		/// <summary>
		/// 実行処理
		/// </summary>
		public override void Execute() {
			if (!Enable)
				return;
			haveLimit_.Execute(this);

			haveDeploy_.Execute(this);
			haveThink_.Execute(this);
		}
		/// <summary>
		/// 前部列入れ替え
		/// </summary>
		public void ForeSwap() {
			//条件を満たしていないときは無視
			if (IsDisableForeSwap())
				return;

			//先に入れ替えて後方限界ラインを更新する
			ExchangeSquad(Abreast.First, Abreast.Second);
			CalcBackRankDepth();
			haveDeploy_.ForeSwap(this);
		}
		/// <summary>
		/// 後部列入れ替え
		/// </summary>
		public void AftSwap() {
			//条件を満たしていないときは無視
			if (IsDisableAftSwap())
				return;
			//先に入れ替えて後方限界ラインを更新する
			ExchangeSquad(Abreast.Second, Abreast.Third);
			CalcBackRankDepth();
			haveDeploy_.AftSwap(this);
		}

		/// <summary>
		/// 順次列入れ替え
		/// </summary>
		public void Rotation() {
			//条件を満たしていないときは無視
			if (IsDisableRotation())
				return;

			ExchangeSquad(Abreast.First, Abreast.Second);
			ExchangeSquad(Abreast.Second, Abreast.Third);
			CalcBackRankDepth();
			haveDeploy_.Rotation(this);
		}
		/// <summary>
		/// 全体範囲よりも前にいるか
		/// </summary>
		public bool IsForward(float z, float range = 0f) {
			return BattleUtil.IsForward(haveLimit_.ForwardLimit, z, TowardSign, range);
		}
		/// <summary>
		/// 全体範囲よりも後ろにいるか
		/// </summary>
		public bool IsBackward(float z, float range = 0f) {
			return BattleUtil.IsBackward(haveLimit_.BackwardLimit, z, TowardSign, range);
		}

		/// <summary>
		/// 入れ替え中か
		/// </summary>
		public bool IsSwap() {
			//キャラ全員のスワップ状態をチェック
			for (int i = 0, max = squads_.Count; i < max; i++) {
				if (squads_[i].IsSwap())
					return true;
			}
			return false;
		}

		/// <summary>
		/// 前列入れ替え可能な状態か
		/// </summary>
		public bool IsDisableForeSwap() {
			//1列しか残ってない場合は操作不能
			if (squads_.Count < 2)
				return true;
			//ラッシュ中は操作不能
			if (HaveBlackboard.RushLine)
				return true;

			SquadEntity first = squads_[(int)Abreast.First];
			SquadEntity second = squads_[(int)Abreast.Second];
			//十分な距離が空いてない場合は操作不能
			if (!BattleUtil.IsBackward(first.HaveLimit.BackwardLimit, second.HaveLimit.ForwardLimit, TowardSign, GameConst.Battle.ENABLE_SWAP_DISTANCE))
				return true;
			return IsSwap();
		}

		/// <summary>
		/// 後列入れ替え可能な状態か
		/// </summary>
		public bool IsDisableAftSwap() {
			//2列以下しか残ってない場合は操作不能
			if (squads_.Count < 3)
				return true;
			//ラッシュ中は操作不能
			if (HaveBlackboard.RushLine)
				return true;

			SquadEntity second = squads_[(int)Abreast.Second];
			SquadEntity third = squads_[(int)Abreast.Third];
			//十分な距離が空いてない場合は操作不能
			if (!BattleUtil.IsBackward(second.HaveLimit.BackwardLimit, third.HaveLimit.ForwardLimit, GameConst.Battle.ENABLE_SWAP_DISTANCE))
				return true;
			return IsSwap();
		}
		/// <summary>
		/// ローテーション可能な状態か
		/// </summary>
		public bool IsDisableRotation() {
			//2列以下しか残ってない場合は操作不能
			if (squads_.Count < 3)
				return true;
			//ラッシュ中は操作不能
			if (HaveBlackboard.RushLine)
				return true;
			return IsSwap();
		}

		/// <summary>
		/// 列が全滅
		/// </summary>
		public void Defeat(SquadEntity squad) {
			for (int i = (int)squad.Orbat + 1, max = squads_.Count; i < max; i++) {
				ExchangeSquad((Abreast)(i - 1), (Abreast)i);
			}
			int lastIndex = squads_.Count - 1;
			defeatSquads_.Add(squads_[lastIndex]);
			squads_.RemoveAt(lastIndex);
			Abort();
		}

		/// <summary>
		/// 列入れ替えを中断
		/// </summary>
		private void Abort() {
			for(int i = 0, max = squads_.Count; i < max; i++){
				squads_[i].AbortSwap();
			}
		}


		/// <summary>
		/// グループを交換
		/// </summary>
		private void ExchangeSquad(Abreast alpha, Abreast bravo) {
			if (squads_.Count <= 1)
				return;
			SquadEntity squadAlpha = squads_[(int)alpha];
			SquadEntity squadBravo = squads_[(int)bravo];

			//グループの順番入れ替え
			squads_[(int)alpha] = squadBravo;
			squadBravo.UpdateOrbat(alpha);

			squads_[(int)bravo] = squadAlpha;
			squadAlpha.UpdateOrbat(bravo);
		}

		/// <summary>
		/// 小隊のデータを作る
		/// </summary>
		public PlatoonData ConvertData() {
			PlatoonData platoon = new PlatoonData();

			platoon.id = Id;
			platoon.name = PlatoonName;
			platoon.experience = Experience;

			platoon.squads = new List<SquadData>();
			for(int i = 0, max = squads_.Count; i < max; i++) {
				SquadData squad = squads_[i].ConvertData();
				platoon.squads.Add(squad);
			}

			return platoon;
		}


#if UNITY_EDITOR
	    /// <summary>
	    /// エディタ確認用描画
	    /// </summary>
		public void DrawGizmos(float fieldWidth) {
		    for (int i = 0, max = backRank_.Count; i < max; i++) {
			    Gizmos.DrawLine(new Vector3(-fieldWidth, 0.2f, backRank_[i]), new Vector3(fieldWidth, 0.2f, backRank_[i]));
		    }
	    }
#endif
	}


}
