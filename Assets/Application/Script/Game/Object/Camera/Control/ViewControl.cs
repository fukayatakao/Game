using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// ステージ観賞用カメラ
	/// </summary>
	public class ViewControl : ICameraControl, IHaveControl {
		[SerializeField]
        Vector3 target_;
		[SerializeField]
        Vector3 freezePosition_;
		[SerializeField]
		float minX_;
		[SerializeField]
		float maxX_;
		[SerializeField]
		float minZ_;
		[SerializeField]
		float maxZ_;

		public void SetTarget(Vector3 target) {
	        target_ = target;
        }

		public void SetTargetLimit(float min_x, float max_x, float min_z, float max_z) {
			minX_ = min_x;
			maxX_ = max_x;
			minZ_ = min_z;
			maxZ_ = max_z;
		}


		protected bool enable_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewControl() {
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this);
        }

        // 優先順位
        public int Priority { get{ return (int)OperationPriority.CameraViewing; } }

		/// <summary>
		/// 割り込みチェック
		/// </summary>
		public bool Interrupt() {
			return false;
		}
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
        /// 更新処理
        /// </summary>
        public override void Execute(CameraControlSetting setting) {
            if (!enable_){
                Calculate(target_, setting.Length, setting.ElevationAngle, setting.AzimthAngle);
                return;
            }
            //最初にタッチした指で座標操作
            if (Gesture.IsTouchDown(0)) {
                freezePosition_ = target_;
            }
            if (Gesture.IsSwipe(0)) {
                Vector2 delta = VirtualScreen.PixelToScreenDelta(Gesture.GetSwipeDirect(0));
                target_ = freezePosition_ - MathUtil.RotateEulerY(setting.AzimthAngle) * (new Vector3(delta.x, 0f, delta.y) * 0.01f) * setting.DisntaceScale;

				target_.x = Mathf.Clamp(target_.x, minX_, maxX_);
				target_.z = Mathf.Clamp(target_.z, minZ_, maxZ_);
			}
			//２番目にタッチした指で回転操作
			if (Gesture.IsSwipe(1)) {
                Vector2 delta = VirtualScreen.PixelToScreenDelta(Gesture.GetSwipeDeltaPos(1));

                setting.ElevationAngle += delta.y * 0.1f;
                setting.AzimthAngle += delta.x * 0.1f;
            }

            //ピンチで距離操作
            if (Gesture.IsPinchIn || Gesture.IsPinchOut) {
                setting.Length = setting.Length + Gesture.PinchDeltaDistance * setting.DisntaceScale;
            }
            Calculate(target_, setting.Length, setting.ElevationAngle, setting.AzimthAngle);

        }
    }

}
