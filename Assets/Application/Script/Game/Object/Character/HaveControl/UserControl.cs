using Project.Lib;
using UnityEngine;

namespace Project.Game {
	public class UserControl : IHaveControl {
        //カメラの向いている方位角のクォータニオン置き場。カメラの処理で書き換えが行われる
        //public static Quaternion CameraAzimth = Quaternion.identity;

        CameraEntity camera_;

		float MaxStrength = 10f;

        CharacterEntity owner_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserControl(CharacterEntity owner) {
			owner_ = owner;
			SystemMessage.RegisterControl.Broadcast(this);
        }

        public void SetCameraEntity(CameraEntity cam) {
            camera_ = cam;
        }

		/// <summary>
		/// 割り込みチェック
		/// </summary>
		public bool Interrupt() {
			return true;
		}


		protected bool enable_;
        // 優先順位
        public int Priority { get { return (int)OperationPriority.CharacterMove; } }
        /// <summary>
        /// 制御開始
        /// </summary>
        public void Begin() {
            enable_ = true;
        }
        /// <summary>
        /// 制御終了
        /// </summary>
        public bool IsEnd() {
            return false;
        }
        /// <summary>
        /// 制御リクエストを却下された
        /// </summary>
        public void Reject() {
            enable_ = false;
        }


        /// <summary>
        /// 実行処理
        /// </summary>
		public void Execute(){
			owner_.Grounding();


			if (!enable_)
                return;

			Quaternion direct;
			float length;
			//操作がない場合は何もしない
			if (TouchSystem.Controller.Strength == 0f) {
				if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) {
					if (!owner_.HaveAnimation.IsPlay(BattleMotion.Idle)) {
						owner_.HaveAnimation.Play(BattleMotion.Idle);
					}
					return;
				}
				KeyMove(out length, out direct);
			} else {
				TouchMove(out length, out direct);
			}
			owner_.SetRotation(direct);
            float speed = length * MaxStrength * Time.deltaTime;

            owner_.ControllerMove (direct * camera_.Azimth, speed);
			if (!owner_.HaveAnimation.IsPlay(BattleMotion.Run))
				owner_.HaveAnimation.Play(BattleMotion.Run);
		}
		private void KeyMove(out float length, out Quaternion direct) {
			length = 1f;
			int x = 0;
			int z = 0;

			if (Input.GetKey(KeyCode.W)) 
				z++;
			if (Input.GetKey(KeyCode.S))
				z--;
			if (Input.GetKey(KeyCode.A))
				x--;
			if (Input.GetKey(KeyCode.D)) 
				x++;

			direct = MathUtil.LookAtY(new Vector3(x, 0, z));
		}

		private void TouchMove(out float length, out Quaternion direct) {
			length = TouchSystem.Controller.Strength;
			direct = TouchSystem.Controller.Direction;
		}

	}
}
