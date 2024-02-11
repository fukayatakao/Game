using UnityEngine;
using System.Collections;
using Project.Lib;


namespace Project.Game {
    /// <summary>
    /// キャラ操作
    /// </summary>
    public class CharacterSelectControl : IHaveControl {
		//カメラ
		CameraEntity camera_;
        //操作中か
        bool enable_;
		//矢印UI
		SpriteRenderer arrow_;
		//前回選択キャラ
		CharacterEntity lastCharacter_;
		//選択キャラ
		CharacterEntity actor_;
		CharacterEntity target_;

		CoroutineTask task_ = new CoroutineTask();

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
		public CharacterSelectControl(CameraEntity cam, Transform ui3d) {
			//インスタンス作って非表示にしておく
			arrow_ = UnityUtil.InstantiateChild(ui3d, ResourceCache.Load<GameObject>(ResourcesPath.MOVE_ARROW_PREFAB)).GetComponent<SpriteRenderer>();
			arrow_.enabled = false;
			camera_ = cam;
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this);

			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (!Gesture.IsTouchDown(0))
				return false;
			//前回入替が終わってない内は新たな操作を受け付けない
			if (!task_.IsEnd())
				return false;
			//キャラクターを選択したら操作開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
			if (!result)
				return false;

			actor_ = hit.collider.GetComponent<CharacterPortal>().Owner;
			if (actor_.IsLeader)
				return false;
			if (lastCharacter_ != null)
				lastCharacter_.UnSelectCharacter();
			actor_.SelectCharacter();
			lastCharacter_ = actor_;
			target_ = null;
			OrganizeMessage.SelectCharacter.Broadcast(actor_, target_);

			return true;
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
			arrow_.enabled = true;
			enable_ = true;
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
			endControl();
			enable_ = false;

		}

        /// <summary>
        /// 実行処理
        /// </summary>
        public void Execute() {
			task_.Execute();

			if (!enable_)
                return;

			//タッチ操作中
			if (Gesture.IsTouch()) {
				//タッチ位置に合わせて矢印を延ばす
				RaycastHit hit;
				bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Field, out hit);
				if (result) {
					Vector3 vec = hit.point - actor_.GetPosition();
					arrow_.transform.localPosition = actor_.GetPosition() + DefaultOffset;
					arrow_.transform.localRotation = MathUtil.LookAtY(vec) * DefaultRotate;
					arrow_.size = new Vector2(DefaultArrowSize.x, DefaultArrowSize.y + vec.magnitude - 0.5f);
				}

				result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
				if (result) {
					Target(hit.collider.GetComponent<CharacterPortal>().Owner);
				} else {
					UnTarget();
				}

				OrganizeMessage.SelectCharacter.Broadcast(actor_, target_);

			//操作終わり
			} else if (Gesture.IsTouchUp()) {
				//キャラを指定座標まで移動させる
				RaycastHit hit;
				bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
				if (result) {
					CharacterEntity target = hit.collider.GetComponent<CharacterPortal>().Owner;

					//種族が一致しない場合は移動不可
					if(actor_ != target && OrganizeUtil.IsExChangeable(actor_, target)) {
						actor_.OrderRelocation(target);
						target.OrderRelocation(actor_);
						OrganizeMessage.SwapCharacter.Broadcast(actor_, target);

						//@todo アセットの非同期読み込みにしたせいでエラーが出てしまうのでそのうち直す
						//種族が一致するが兵科が違う場合はモデル変え
						if(actor_.HaveUnitMaster.Id != target.HaveUnitMaster.Id) {

							//Masterは入替の過程で変えられるので一応変数に入れてから変える
							var mst = actor_.HaveUnitMaster;
							CharacterTask.BuildChangeSwitch(actor_, target.HaveUnitMaster);
							CharacterTask.BuildChangeSwitch(target, mst);

						}
						task_.Play(Working(actor_, target));
					}


				}
				OrganizeMessage.SelectCharacter.Broadcast(actor_, null);
				endControl();
				enable_ = false;
			}
		}

		/// <summary>
		/// 前回選択したキャラ情報をクリア
		/// </summary>
		public void UnSelectLastTarget() {
			lastCharacter_ = null;
		}

		/// <summary>
		/// 操作終了処理
		/// </summary>
		private void endControl() {
			UnTarget();
			arrow_.enabled = false;
			arrow_.size = Vector2.zero;

			actor_ = null;
		}

		/// <summary>
		/// キャラ入替が終わったかチェック
		/// </summary>
		private IEnumerator Working(CharacterEntity actor, CharacterEntity target) {


			//移動するキャラが目的地に到着するまで待機
			while (!actor.HavePosition.CheckDeployPos(actor.GetPosition()))
				yield return null;
			while (!target.HavePosition.CheckDeployPos(target.GetPosition()))
				yield return null;




			yield break;
		}

		private void Target(CharacterEntity target) {
			if (target != actor_) {
				UnTarget();
				target.TargetCharacter();
				target_ = target;
			} else {
				UnTarget();
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
