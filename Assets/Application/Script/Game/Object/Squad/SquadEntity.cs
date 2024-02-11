using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// 分隊
	/// </summary>
	/// <remarks>
	/// １列に並んだキャラクター全体の制御クラス
	/// </remarks>
	public class SquadEntity : Entity {
		//所属
		public PlatoonEntity Platoon { get; private set; }
		//小隊作成時に決まって変化しない序列
		public Abreast Index { get; private set; }
		//列入れ替えで変化する序列
		public Abreast Orbat { get; private set; }
		//ユニットデータ
		public MstUnitData UnitData { get; private set; }
		//一般ユニットが1体以上いるかリーダーが居てまだ生きている場合は分隊生存してる
		public bool IsAlive { get { return IsActive() && ((members_ != null && members_.Count > 0) || (leader_ != null && leader_.IsAlive)); } }

		//生存キャラリスト
		private List<CharacterEntity> members_;
		public IReadOnlyList<CharacterEntity> Members { get { return members_; } }

		private CharacterEntity leader_;
		public CharacterEntity Leader { get { return leader_; } }

		//死亡キャラリスト
		private List<CharacterEntity> defeatMembers_ = new List<CharacterEntity>();
		public IReadOnlyList<CharacterEntity> DefeatMembers { get { return defeatMembers_; } }

		private CharacterEntity defeatLeader_;
		public CharacterEntity DefeatLeader { get { return defeatLeader_; } }

		//サブモジュール系
		//記憶領域
		SquadLimit haveLimit_;
		public SquadLimit HaveLimit { get { return haveLimit_; } }
		//パラメータ
		SquadParam haveParam_;
		public SquadParam HaveParam { get { return haveParam_; } }
		//展開ライン
		SquadDeployLine haveDeployLine_;
		public SquadDeployLine HaveDeployLine { get { return haveDeployLine_; } }
#if false
		SquadCollision haveCollision_;
		public SquadCollision HaveCollision { get { return haveCollision_; } }
#endif

		//メッセージシステム用
		MessageSystem.Receptor receptor_;


		/// <summary>
		/// インスタンス生成処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = new GameObject(resName);

			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent);
			haveLimit_ = MonoPretender.Create<SquadLimit>(obj);
			haveParam_ = MonoPretender.Create<SquadParam>(obj);
			haveDeployLine_ = MonoPretender.Create<SquadDeployLine>(obj);


			//展開するべきラインのインスタンスを生成
			await haveDeployLine_.LoadAsync(cacheTrans_);

			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);
			return obj;
		}

		/// <summary>
		/// 所属キャラクター生成
		/// </summary>
		public async Task CreateMember(MstUnitData mstUnit, List<Network.CharacterData> members, bool isImmediate) {
			//無効ユニットなのに所属キャラがいる
			Debug.Assert(!(mstUnit.Id == 0 && members.Count != 0), "invalid unit id:" + mstUnit.Id);
			//ユニット指定がされているのに所属キャラがいない
			Debug.Assert(!(mstUnit.Id != 0 && members.Count == 0), "no members");
			Debug.Assert(members_ == null, "already create instance");
			members_ = new List<CharacterEntity>();
			//キャラクターのインスタンスを生成
			for (int i = 0, max = members.Count; i < max; i++) {
				CharacterEntity entity = await CharacterAssembly.I.CreateAsync(mstUnit.ModelName, mstUnit.AnimationLabel, mstUnit.ActionLabel, mstUnit.AiName, isImmediate);
				entity.Setup(mstUnit, members[i], cacheTrans_);
				members_.Add(entity);
			}
			UnitData = mstUnit;
			defeatMembers_.Clear();
		}

		/// <summary>
		/// 所属リーダー生成
		/// </summary>
		public async Task CreateLeader(Network.LeaderData leader, bool isImmediate) {
			Debug.Assert(leader_ == null, "already create instance");
			//キャラクターのインスタンスを生成
			MstUnitData mst = MstLeaderData.GetData(leader.leaderMasterId).UnitData;
			leader_ = await CharacterAssembly.I.CreateAsync(mst.ModelName, mst.AnimationLabel, mst.ActionLabel, mst.AiName, isImmediate);
			leader_.Setup(mst, leader, cacheTrans_);
			defeatLeader_ = null;
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(haveLimit_);
			MonoPretender.Destroy(haveParam_);
			MonoPretender.Destroy(haveDeployLine_);

			MessageSystem.DestroyReceptor(receptor_);
		}

		/// <summary>
		/// 初期配置処理
		/// </summary>
		public void InitLocation(float depth, float sign, bool isDistortion) {
			if (isDistortion) {
				haveDeployLine_.UpdateLine(depth, sign);
			} else {
				haveDeployLine_.InitDepth(depth);
			}

			//一般ユニットが配備されている場合
			if(members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					Vector3 pos = haveDeployLine_.CalcUnitPos(i, max);
					members_[i].InitLocation(pos, sign);
				}
			}

			//リーダーが配備されている場合
			if (leader_ != null) {
				//列キャラの最大半径を取得（ユニットは現在は全部同一なので1体だけ調べればいいのだけど混成される場合に備えてちゃんと全キャラ調べる
				float r = 0f;
				if (members_ != null) {
					for (int i = 0, max = members_.Count; i < max; i++) {
						r = r > members_[i].HaveCollision.Radius ? r : members_[i].HaveCollision.Radius;
					}
				}

				haveDeployLine_.InitOffset(leader_, r, sign);
				leader_.InitLocation(haveDeployLine_.CalcLeaderPos(), sign);
			}
		}

		/// <summary>
		/// 所属情報を初期化
		/// </summary>
		public void InitAttitude(PlatoonEntity platoon, Abreast abreast) {
			Platoon = platoon;
			Index = abreast;
			Orbat = abreast;
			if (members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					members_[i].InitAttitude(this);
				}
			}
			leader_?.InitAttitude(this);

		}
		/// <summary>
		/// 展開ラインの設定
		/// </summary>
		public void InitLine(float width, float sign) {
			haveDeployLine_.Init(width);
		}

		/// <summary>
		/// 熟練度ボーナス設定
		/// </summary>
		public void InitParam(int experience) {
			//トータルHP,LPを計算して記録する
			haveParam_.Setup(this);
			for (int i = 0, max = members_.Count; i < max; i++) {
				BattleCombat.AddExperienceBonus(members_[i], experience);
			}
		}

		/// <summary>
		/// UI設定初期化
		/// </summary>
		public void InitUI(CameraEntity cameraEntity, Transform canvas) {
			if (members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					members_[i].InitUI(cameraEntity, canvas);
				}
			}

			if (leader_ != null) {
				leader_.InitUI(cameraEntity, canvas);
			}
		}
		/// <summary>
		/// 所属キャラを全削除
		/// </summary>
		public void DestroyAllMembers() {
			if (members_ == null)
				return;
			//既存のキャラを全削除
			for (int i = 0, max = members_.Count; i < max; i++) {
				//インスタンスの破棄
				CharacterAssembly.I.Destroy(members_[i]);
			}
			members_ = null;
			UnitData = null;
		}

		/// <summary>
		/// 所属リーダーを削除
		/// </summary>
		public void DestroyLeader() {
			if (leader_ == null)
				return;
			CharacterAssembly.I.Destroy(leader_);
			leader_ = null;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public override void Execute() {
			//分隊が全滅していた場合は処理しない
			if (!IsAlive) {
				return;
			}

			haveLimit_.Execute(this);
			haveParam_.Execute(this);
		}
		/// <summary>
		/// キャラ削除
		/// </summary>
		public void Defeat(CharacterEntity character) {
			//リーダーの場合
			if (character.IsLeader) {
				leader_ = null;
				defeatLeader_ = character;
			} else {
				members_.Remove(character);
				defeatMembers_.Add(character);
			}
			haveParam_.Apply(this);

			//分隊全滅した場合
			if(!IsAlive) {
				haveParam_.Defeat();
				Platoon.Defeat(this);
				//小隊全滅した場合はゲーム終了
				if (!Platoon.IsAlive) {
					BattleMessage.GameEnd.Broadcast();
				}
			}

		}
		/// <summary>
		/// 入れ替え中か
		/// </summary>
		public bool IsSwap() {
			if(members_ != null) {
				//キャラ全員のスワップ状態をチェック
				for (int i = 0, max = members_.Count; i < max; i++) {
					if (members_[i].HaveBlackboard.SwapBoard.IsSwap) {
						return true;
					}
				}
			}

			if (leader_ != null)
				return leader_.HaveBlackboard.SwapBoard.IsSwap;
			return false;
		}
		/// <summary>
		/// 列範囲よりも前にいるか
		/// </summary>
		public bool IsForward(float z, float range = 0f) {
			return BattleUtil.IsForward(haveLimit_.ForwardLimit, z, Platoon.TowardSign, range);
		}
		/// <summary>
		/// 列範囲よりも後ろにいるか
		/// </summary>
		public bool IsBackward(float z, float range = 0f) {
			return BattleUtil.IsBackward(haveLimit_.BackwardLimit, z, Platoon.TowardSign, range);
		}
		/// <summary>
		/// 展開ラインの更新
		/// </summary>
		public void UpdateLine(float depth) {
			haveDeployLine_.UpdateLine(depth, Platoon.TowardSign);
			if (members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					members_[i].SetDeployPos(haveDeployLine_.CalcUnitPos(i, max));
				}
			}
			leader_?.SetDeployPos(haveDeployLine_.CalcLeaderPos());
		}
		/// <summary>
		/// ユニットを置き換える
		/// </summary>
		public void ChangeCharacter(int index, CharacterEntity chara) {
			members_[index] = chara;
			chara.InitAttitude(this);
		}


		/// <summary>
		/// 後ろに移動して交代
		/// </summary>
		public void SwapBackward(SquadEntity target) {
			if (members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					members_[i].HaveBlackboard.SwapBoard.SwapTarget = target;
					members_[i].ChangeAIState(CharacterThink.State.Backward);
				}
			}

			if (leader_ != null) {
				leader_.HaveBlackboard.SwapBoard.SwapTarget = target;
				leader_.ChangeAIState(CharacterThink.State.Backward);
			}
		}

		/// <summary>
		///
		/// </summary>
		public void AbortSwap() {
			if (members_ != null) {
				for (int i = 0, max = members_.Count; i < max; i++) {
					members_[i].HaveBlackboard.SwapBoard.SwapTarget = null;
				}
			}

			if (leader_ != null) {
				leader_.HaveBlackboard.SwapBoard.SwapTarget = null;
			}
		}

		/// <summary>
		/// 戦闘序列が変わったら更新する
		/// </summary>
		public void UpdateOrbat(Abreast orbat) {
			Orbat = orbat;
		}

		/// <summary>
		/// 分隊のデータを作る
		/// </summary>
		public SquadData ConvertData() {
			SquadData data = new SquadData();

			data.members = new List<CharacterData>();
			//一般ユニットのデータ
			if (members_ != null) {
				data.unitId = UnitData.Id;
				for (int i = 0, max = members_.Count; i < max; i++) {
					if (members_[i].Id < 0)
						data.members.Add(null);
					else
						data.members.Add(members_[i].ConvertCharacterData());
				}
			}

			//リーダーがいたらリーダーのデータも作る
			if (leader_ != null) {
				data.leader = leader_.ConvertLeaderData();
			}
			return data;
		}
		/// <summary>
		/// 前衛適性値を計算
		/// </summary>
		public float CalcAptitude() {
			//@note 暫定で適性x総HP量を前衛適性とする
			return UnitData.Forward * haveParam_.Hp;
		}
	}


}
