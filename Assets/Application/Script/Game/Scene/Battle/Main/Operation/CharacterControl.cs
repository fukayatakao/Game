using UnityEngine;
using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// キャラ操作
    /// </summary>
    public class CharacterControl : IHaveControl {
		//カメラ
		CameraEntity camera_;
        //対象のキャラクター
        CharacterEntity actor_;
		CharacterEntity target_;

		//矢印UI
		SpriteRenderer arrow_;
        //操作中か
        bool enable_;

        //矢印UIのデフォルトサイズ
        static readonly Vector2 DefaultArrowSize = new Vector2(0.5f, 0.5f);
        //地面から浮かすオフセット
        static readonly Vector3 DefaultOffset = new Vector3(0f, 0.05f, 0f);
        //地面に水平になるように回転
        static readonly Quaternion DefaultRotate = MathUtil.RotateEulerX(-90f);

        // 操作プライオリティ
        public int Priority { get { return (int)OperationPriority.CharacterDrag; } }


		/// <summary>
		/// レイを飛ばすカメラとUIをセットアップ
		/// </summary>
		public CharacterControl(CameraEntity cam, Transform ui3d) {
			//インスタンス作って非表示にしておく
			arrow_ = UnityUtil.InstantiateChild(ui3d, ResourceCache.Load<GameObject>(ResourcesPath.MOVE_ARROW_PREFAB)).GetComponent<SpriteRenderer>();
			arrow_.enabled = false;
			camera_ = cam;
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (!Gesture.IsTouchDown(0))
				return false;

			//キャラクターを選択したら操作開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
			if (!result)
				return false;

			CharacterEntity entity = hit.collider.GetComponent<CharacterPortal>().Owner;
			//プレイヤー以外のキャラだった場合無視
			if (entity.Platoon.Index != Power.Player) {
				return false;
			}
			actor_ = entity;
			target_ = null;


			return true;
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
			Enter();
		}
        /// <summary>
        /// 制御終了
        /// </summary>
        public bool IsEnd() {
            return !enable_;
        }
        /// <summary>
        /// 制御却下
        /// </summary>
        public void Reject() {
			Exit();
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
            if (!enable_)
                return;

			//タッチ操作中
			if (Gesture.IsTouch()) {
				//タッチ位置に合わせて矢印を延ばす
				RaycastHit[] hit = CameraUtil.RaycastHitAll(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Field | 1 << (int)UnityLayer.Layer.Character);
				UnTarget();
				for (int i = 0, max = hit.Length; i < max; i++) {
					if (hit[i].collider.gameObject.layer == (int)UnityLayer.Layer.Field) {
						Vector3 vec = CalcGroundTargetVector(hit[i]);
						arrow_.transform.localPosition = actor_.GetPosition() + DefaultOffset;
						arrow_.transform.localRotation = MathUtil.LookAtY(vec) * DefaultRotate;
						arrow_.size = new Vector2(DefaultArrowSize.x, DefaultArrowSize.y + vec.magnitude - 0.5f);
						continue;
					}
					if (hit[i].collider.gameObject.layer == (int)UnityLayer.Layer.Character) {
						CharacterEntity target = hit[i].collider.GetComponent<CharacterPortal>().Owner;
						Target(target);
						continue;
					}
				}

			//操作終わり
			} else if (Gesture.IsTouchUp()) {
				//キャラを指定座標まで移動させる
				RaycastHit[] hit = CameraUtil.RaycastHitAll(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character | 1 << (int)UnityLayer.Layer.Collision);
				if (!LockonEnemy(hit)) {
					GroundTargetMove(hit);
				}

				Exit();
			}
		}
		/// <summary>
		/// 操作開始
		/// </summary>
		private void Enter() {
			TimeControl.Lock();
			arrow_.enabled = true;
			enable_ = true;
			arrow_.size = new Vector2();

			actor_.HaveOutline.Select();
		}

		/// <summary>
		/// 操作終了
		/// </summary>
		private void Exit() {
			TimeControl.UnLock();
			arrow_.enabled = false;
			enable_ = false;
			actor_.HaveOutline.UnSelect();
			UnTarget();
			target_ = null;
		}

		/// <summary>
		/// 敵キャラクターを追跡する
		/// </summary>
		private bool LockonEnemy(RaycastHit[] hit) {
			//キャラクターを選択しているか調べる
			for (int i = 0, max = hit.Length; i < max; i++) {
				if (hit[i].collider.gameObject.layer != (int)UnityLayer.Layer.Character)
					continue;

				CharacterEntity target = hit[i].collider.GetComponent<CharacterPortal>().Owner;
				//選択したのが味方の場合は無視
				if (target.Platoon.Index == actor_.Platoon.Index)
					continue;
				//敵を追いかけて近づいたら攻撃に移る
				actor_.OrderChase(target);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 特定の座標へ移動する
		/// </summary>
		private bool GroundTargetMove(RaycastHit[] hit) {
			//ステージとの交差を調べる
			for (int i = 0, max = hit.Length; i < max; i++) {
				if (hit[i].collider.gameObject.layer != (int)UnityLayer.Layer.Field)
					continue;

				Vector3 vec = CalcGroundTargetVector(hit[i]);
				//敵を追いかけて近づいたら攻撃に移る
				actor_.OrderGroundTarget(vec);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 指定地点移動は一定以上離れていたら距離を詰める
		/// </summary>
		private Vector3 CalcGroundTargetVector(RaycastHit hit) {
			Vector3 vec = hit.point - actor_.GetPosition();
			if (vec.magnitude > GameConst.Battle.GROUND_TARGET_DISTANCE) {
				vec = vec.normalized * GameConst.Battle.GROUND_TARGET_DISTANCE;
			}
			return vec;
		}

		private void Target(CharacterEntity target) {
			//違うチームの場合のみ処理
			if (target.Platoon.Index != actor_.Platoon.Index) {
				target.TargetCharacter();
				target_ = target;
			}
		}
		private void UnTarget() {
			if (target_ != null) {
				target_.UnTargetCharacter();
				target_ = null;
			}
		}

	}
}
