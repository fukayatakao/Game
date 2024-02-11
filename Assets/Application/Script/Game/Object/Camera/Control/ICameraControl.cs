using UnityEngine;
using Project.Lib;


namespace Project.Game {
	public abstract class ICameraControl : MonoPretender {
		//カメラ座標
		protected Vector3 position_;
		public Vector3 Position{ get { return position_; } }
		//カメラの回転
		protected Quaternion rotation_ = Quaternion.identity;
		public Quaternion Rotation{ get { return rotation_; } }

		public abstract void Execute(CameraControlSetting setting);


        //視線ベクトル
        /*protected static float length_;
        public static float Length {
            set { elevationAngle_ = value; if (elevationAngle_ < 0f) elevationAngle_ = 0f; if (elevationAngle_ > 90f) elevationAngle_ = 90f; }
            get { return elevationAngle_; }
        }
        //仰角
        private static float elevationAngle_;
        public static float elevationAngle{
            set { elevationAngle_ = value; if (elevationAngle_ < 0f) elevationAngle_ = 0f; if (elevationAngle_ > 90f) elevationAngle_ = 90f; }
            get { return elevationAngle_; }
        }
        //方位角
        private static float azimthAngle_;
        public static float azimthAngle {
            set { azimthAngle_ = value; if (azimthAngle_ < -180f) azimthAngle_ = -180f; if (azimthAngle_ > 180f) azimthAngle_ = 180f; }
            get { return azimthAngle_; }
        }

        /// <summary>
        /// 初期化
        /// </summary>
		public static void Setup(float len, float e, float a = 0f) {
            elevationAngle = e;
            azimthAngle = a;
            length_ = len;

		}*/

        /// <summary>
        /// 仰角・方位角・注視点でカメラ制御
        /// </summary>
        /// <param name="pos"></param>
        protected void Calculate(Vector3 target, float length, float elevation, float azimth) {
            rotation_ = MathUtil.RotateEulerY(azimth) * MathUtil.RotateEulerX(elevation);
            position_ = rotation_ * (Vector3.forward * -length) + target;
        }
    }
}
