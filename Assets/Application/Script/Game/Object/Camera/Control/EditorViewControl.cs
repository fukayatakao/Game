using Project.Lib;
using UnityEngine;

#if UNITY_EDITOR
namespace Project.Game {
	/// <summary>
	/// エディタビュー用カメラ
	/// </summary>
	public class EditorViewControl : ICameraControl, IHaveControl {
		[SerializeField]
        Vector3 target_;
		[SerializeField]
        Vector3 freezePosition_;

		private static readonly Vector3 defaultTarget = new Vector3(0f, 0.5f, 0f);
		private const float defaultAzimth = 180f;
		private const float defaultElevation = 10f;
		private const float defaultLength = 1.5f;

		public static float TargetMoveRatio = 0.0005f;
		protected bool enable_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditorViewControl() {
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this);
        }

        // 優先順位
        public int Priority { get{ return (int)OperationPriority.CameraEditorViewing; } }

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
		/// カメラの設定値をリセット
		/// </summary>
		public void Reset(CameraControlSetting setting, bool isPos=true, bool isRot=true, bool isLen=true) {
			if (isPos) {
				target_ = defaultTarget;
			}
			if (isRot) {
				setting.ElevationAngle = defaultElevation;
				setting.AzimthAngle = defaultAzimth;
			}
			if (isLen) {
				setting.Length = defaultLength;
			}
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
			if (Gesture.IsTouchDown(1)) {
                freezePosition_ = target_;
            }
            if (Gesture.IsSwipe(1)) {
                Vector2 delta = VirtualScreen.PixelToScreenDelta(Gesture.GetSwipeDirect(1));
                target_ = freezePosition_ - MathUtil.RotateEulerY(setting.AzimthAngle) * (new Vector3(delta.x, delta.y, 0f) * TargetMoveRatio);
                //target_.x = freezePosition_.x - delta.x * 0.01f;
                //target_.z = freezePosition_.z - delta.y * 0.01f;
            }
            //回転操作
            if (Gesture.IsSwipe(0)) {
                Vector2 delta = VirtualScreen.PixelToScreenDelta(Gesture.GetSwipeDeltaPos(0));

                setting.ElevationAngle += delta.y * 0.1f;
                setting.AzimthAngle += delta.x * 0.1f;
            }

            //ピンチで距離操作
            if (Gesture.IsPinchIn || Gesture.IsPinchOut) {
                setting.Length = setting.Length + Gesture.PinchDeltaDistance;
            }
            Calculate(target_, setting.Length, setting.ElevationAngle, setting.AzimthAngle);

        }
    }

}
#endif
