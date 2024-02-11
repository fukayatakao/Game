using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// 注視カメラ
	/// </summary>
	public class LockOnControl : ICameraControl {
		//注視する対象
		Transform aimTarget_;
		//プレイヤー
		Transform followTarget_;




		/// <summary>
		/// 注視するターゲットを設定
		/// </summary>
		/// <param name="target">Target.</param>
		public void SetTarget(Transform player, Transform target){
			followTarget_ = player;
			aimTarget_ = target;
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(CameraControlSetting setting) {
			//注視対象から追随対象へのベクトルを計算(高さは無視する
			Vector3 vec = aimTarget_.localPosition - followTarget_.localPosition;
			//半角定理を使って高速でクォータニオンを直接作る
			Quaternion azimth = MathUtil.LookAtY(vec);



            position_ = azimth * MathUtil.RotateEulerX(setting.ElevationAngle) * (Vector3.forward * -setting.Length) + followTarget_.localPosition;
			rotation_ = azimth * MathUtil.RotateEulerX(setting.ElevationAngle);

		}
	}

}
