using UnityEngine;
using Project.Lib;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 戦場の駒
    /// </summary>
	public class PieceEntity : Entity {
		//管理Id
		public int Id { get { return data.id; } }
		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		//@todo Characterのコンポーネントはそのうち置き換える
		private CharacterModel haveModel_;
		public CharacterModel HaveModel { get { return haveModel_; } }
		//ステート管理
		private PieceState haveState_;
		public PieceState HaveState { get { return haveState_; } }

		//そのフレームでの移動ベクトル
		private Vector3 moveVector_;
        public Vector3 MoveVector { get { return moveVector_; } }

		//戦域マップ用
		public Geopolitics.Way MoveWay;

		public PlatoonData data;

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
			haveModel_.Model.AddComponent<PiecePortal>().Init(this);
			haveModel_.Init();

			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
			haveState_ = MonoPretender.Create<PieceState>(obj);

			//InitModel();
			return obj;
		}

		public void Move(Geopolitics.Way way) {
			MoveWay = way;
			ChangeState(PieceState.State.Move);
		}

		/*/// <summary>
		/// Model表示に関連する初期化
		/// </summary>
		private void InitModel() {
			GameObject model = haveModel_.Model;

			//アニメーション制御クラス生成（初期化はAwakeの中で行われている
			haveAnimation_ = model.AddComponent<CharacterAnimation>();
			haveAnimation_.Create();

			haveOutline_ = MonoPretender.Create<CharacterOutline>(model);

			AttachEvent(model);
		}

		/// <summary>
		/// UnityChanのアセットがAnimationEvent仕込んでいるので無視するために対策
		/// </summary>
		private void AttachEvent(GameObject obj) {
			obj.AddComponent<AnimationEventCallback>();
		}

		public async Task LoadAnimation(string animeName) {
			await haveAnimation_.LoadAsync(animeName);
		}*/

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(haveModel_);
			MonoPretender.Destroy(haveState_);


			MessageSystem.DestroyReceptor(receptor_);
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public override void Execute() {
			haveState_.Execute(this);
		}

		/// <summary>
		/// Playable更新
		/// </summary>
		public override void Evaluate() {
			haveModel_.HaveAnimation.LateExecute();
		}

		/// <summary>
		/// 実行後処理
		/// </summary>
		public override void LateExecute() {
			SetPosition(GetPosition() + moveVector_);

            moveVector_ = Vector3.zero;

			Grounding();
		}

		public void ChangeState(PieceState.State state, bool force = false) {
			HaveState.ChangeState(this, state, force);
		}

		/// <summary>
		/// 接地計算
		/// </summary>
		public void Grounding(float height=10f, float distance=100f) {
			SetPosition(StrategyUtil.Grounding(GetPosition()));
		}

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        public void Move(Quaternion direction, float speed = 1f) {
            moveVector_ += direction * new Vector3(0f, 0f, speed);
        }

		/// <summary>
		/// 存在チェック
		/// </summary>
		public override bool IsExist() {
			//自動インスタンス破棄はしない
			return true;
		}
	}
}
