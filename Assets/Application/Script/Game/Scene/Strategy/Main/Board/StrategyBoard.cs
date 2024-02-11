using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	public class StrategyBoard {
		FieldEntity field_;
		public FieldEntity Field { get { return field_; } }
		//攻撃側予備部隊
		List<PlatoonData> reserve_;
		public List<PlatoonData> Reserve { get { return reserve_; } }
		//マップ上に配置されてる攻撃側部隊(キーはnodeId
		Dictionary<int, PieceEntity> invader_;
		public Dictionary<int, PieceEntity> Invader { get { return invader_; } }

		Dictionary<int, PieceEntity> defender_;
		public Dictionary<int, PieceEntity> Defender { get { return defender_; } }

		int currentTurn_;
		public int CurrentTurn { get { return currentTurn_; } }
		int maxTurn_;
		public int MaxTurn { get { return maxTurn_; } }

		//現ターンで行動可能な駒
		List<int> availableInvader_ = new List<int>();
		public List<int> AvailableInvader { get { return availableInvader_; } }

		MessageSystem.Receptor receptor_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public StrategyBoard() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(int turn, List<int> available) {
			currentTurn_ = turn;
			availableInvader_ = available;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public async Task LoadField(int mapId) {
			MstStrategyMapData data = BaseDataManager.GetDictionary<int, MstStrategyMapData>()[mapId];
			field_ = await FieldAssembly.I.CreateAsync(data.SceneName, "Field/Sky/rustig_koppie_4k", data.WaymapName);
			maxTurn_ = data.MaxTurn;
		}
		/// <summary>
		/// 攻撃側ユニット初期配置
		/// </summary>
		public async Task LocateInvader(List<PlatoonData> platoon, List<StrategyLocationData> location) {
			invader_ = new Dictionary<int, PieceEntity>();

			//locationで配置済の部隊はreserveから削除
			for (int i = 0, max = location.Count; i < max; i++) {
				int locationId = location[i].nodeId;
				PlatoonData data = location[i].platoon;
				int unitId = PickPieceModel(data);
				var mst = BaseDataManager.GetDictionary<int, MstUnitData>()[unitId];
				PieceEntity entity = await PieceAssembly.I.CreateAsync(mst.ModelName, mst.AnimationLabel);

				entity.data = data;
				entity.SetPosition(field_.HaveWaymap.nodeDict[locationId].position);
				entity.SetRotation(Quaternion.Euler(0f, 0f, 0f));
				entity.ChangeState(PieceState.State.Idle);

				invader_[location[i].nodeId] = entity;
			}

			reserve_ = new List<PlatoonData>(platoon);
		}
		//@todo LocateInvaderと共通化する
		/// <summary>
		/// 防御側ユニット配置
		/// </summary>
		public async Task LocateDefender(List<PlatoonData> platoon, List<StrategyLocationData> location) {
			defender_ = new Dictionary<int, PieceEntity>();
			for (int i = 0, max = location.Count; i < max; i++) {
				int locationId = location[i].nodeId;
				PlatoonData data = location[i].platoon;
				int unitId = PickPieceModel(data);
				var mst = BaseDataManager.GetDictionary<int, MstUnitData>()[unitId];
				PieceEntity entity = await PieceAssembly.I.CreateAsync(mst.ModelName, mst.AnimationLabel);

				entity.data = data;
				entity.SetPosition(field_.HaveWaymap.nodeDict[locationId].position);
				entity.SetRotation(Quaternion.Euler(0f, 180f, 0f));
				entity.ChangeState(PieceState.State.Idle);
				defender_[locationId] = entity;
			}
		}

		/// <summary>
		/// 攻撃側ユーザーの配置操作
		/// </summary>
		public async void PutInvader(int nodeId) {
			//既に配置済の場合は無視
			if (invader_.ContainsKey(nodeId))
				return;

			Vector3 pos = field_.HaveWaymap.nodeDict[nodeId].position;
			int unitId = PickPieceModel(reserve_[0]);
			var mst = BaseDataManager.GetDictionary<int, MstUnitData>()[unitId];
			PieceEntity entity = await PieceAssembly.I.CreateAsync(mst.ModelName, mst.AnimationLabel, true);
			entity.SetPosition(pos);
			entity.Grounding();
			entity.ChangeState(PieceState.State.Idle);

			entity.data = reserve_[0];
			reserve_.RemoveAt(0);

			invader_[nodeId] = entity;

			if (invader_.Count >= field_.HaveWaymap.starts.Count || invader_.Count >= StrategySituation.I.invader.Count) {
				NextTurn();
				UpdateServerBoard((response) => { StrategyMessage.StartStrategyMain.Broadcast(); });
			}
		}

		private int PickPieceModel(PlatoonData data) {
			for (int i = data.squads.Count - 1; i >= 0; i--)
			{
				if (data.squads[i].leader != null)
					return data.squads[i].leader.leaderMasterId;
			}

			//リーダーなしでユニットのみの場合は一旦エラー、そのうち汎用ユニット用のモデルを準備すべきか
			Debug.LogError("not found leader");
			return 0;
		}

		/// <summary>
		/// サーバ側に情報を送って盤面情報を更新
		/// </summary>
		public void UpdateServerBoard(System.Action<Response> success) {
			List<StrategyLocationData> invaderLocation = CreateCurrentLocation(invader_);
			List<StrategyLocationData> defenderLocation = CreateCurrentLocation(defender_);


			UpdateBoardCmd.CreateAsync(new UpdateBoardRequest(invaderLocation, defenderLocation, currentTurn_, availableInvader_), success);
		}


		private List<StrategyLocationData> CreateCurrentLocation(Dictionary<int, PieceEntity> party) {
			List<StrategyLocationData> invaderLocation = new List<StrategyLocationData>();

			foreach (var invader in party) {
				StrategyLocationData data = new StrategyLocationData();
				data.platoon = invader.Value.data;
				data.nodeId = invader.Key;
				invaderLocation.Add(data);
			}
			return invaderLocation;
		}

		public void NextTurn() {
			currentTurn_++;
			foreach(var inv in invader_.Values) {
				availableInvader_.Add(inv.Id);
			}

		}

		/// <summary>
		/// 攻撃側の駒を移動させる
		/// </summary>
		public void MovePiece(int last, int current) {
			//今までいた場所のキーを削除して新しい場所のキーに移す
			Invader[current] = Invader[last];
			Invader.Remove(last);

			UpdateServerBoard((response) => { });
		}
	}
}
