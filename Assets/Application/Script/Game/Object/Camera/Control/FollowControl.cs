using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// 追随カメラ
	/// </summary>
	public class FollowControl : ICameraControl {
		//追随ターゲット
		Transform followTarget_;

		/// <summary>
		/// 注視するターゲットを設定
		/// </summary>
		public void SetTarget(Transform target){
			followTarget_ = target;
        }




		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CameraControlSetting setting) {
            Calculate(followTarget_.localPosition, setting.Length, setting.ElevationAngle, setting.AzimthAngle);

        }
    }

}
