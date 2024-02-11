using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// 追随カメラ
	/// </summary>
    /// <remarks>
    /// 注視位置から一定以上離れないとカメラが移動しないバージョン
    /// </remarks>
	public class FollowRangeControl : ICameraControl {
		[SerializeField]
		float range = 0.5f;

		//追随ターゲット
		Transform followTarget_;

		//前回座標
		Vector3 lastTargetPos_;
		/// <summary>
		/// 注視するターゲットを設定
		/// </summary>
		/// <param name="target">Target.</param>
		public void SetTarget(Transform target){
			followTarget_ = target;
			lastTargetPos_ = followTarget_.localPosition;
        }



		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CameraControlSetting setting) {
			Vector3 delta = followTarget_.localPosition - lastTargetPos_;
			delta.y = 0f;
			if (delta.sqrMagnitude <= range * range) {
				return;
			}
			float len = delta.magnitude;
			lastTargetPos_ = lastTargetPos_ + delta.normalized * (len - range);
			lastTargetPos_.y = followTarget_.localPosition.y;


            Calculate(lastTargetPos_, setting.Length, setting.ElevationAngle, setting.AzimthAngle);
        }
    }

}
