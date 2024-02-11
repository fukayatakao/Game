using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// キャラ注視用カメラ
	/// </summary>
	public class FollowViewControl : ICameraControl {
		//追随ターゲット
		[SerializeField]
		Transform followTarget_;

		/// <summary>
		/// 注視するターゲットを設定
		/// </summary>
		public void SetTarget(Transform target) {
			followTarget_ = target;
		}


		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CameraControlSetting setting) {
            //２番目にタッチした指で回転操作
            if (Gesture.IsSwipe(1)) {
                Vector2 delta = VirtualScreen.PixelToScreenDelta(Gesture.GetSwipeDeltaPos(1));

                setting.ElevationAngle += delta.y * 0.1f;
                setting.AzimthAngle += delta.x * 0.1f;
            }

            //ピンチで距離操作
            if (Gesture.IsPinchIn || Gesture.IsPinchOut) {
                setting.Length = setting.Length + Gesture.PinchDeltaDistance;
            }
            Calculate(followTarget_.localPosition, setting.Length, setting.ElevationAngle, setting.AzimthAngle);

        }
    }

}
