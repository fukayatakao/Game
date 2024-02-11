using Project.Lib;
using UnityEngine;


namespace Project.Game {
	public class CameraControlSetting : MonoPretender {
        //カメラ距離：現在値,最小値,最大値
        [SerializeField]
        protected float length_;
        [SerializeField]
        private float minLength_ = 3f;
        [SerializeField]
        private float maxLength_ = 15f;
		//距離を変えるときの操作倍率
		[SerializeField]
		private float distanceScale_ = 1f;
		public float DisntaceScale{ get{ return distanceScale_; } }
        //カメラ距離
        public float Length {
            set { length_ = Mathf.Clamp(value, minLength_, maxLength_); }
            get { return length_; }
        }
        //仰角：現在値,最小値,最大値
        [SerializeField]
        private float elevationAngle_;
        [SerializeField]
        private float minElevationAngle_ = 10f;
        [SerializeField]
        private float maxElevationAngle_ = 45f;

        //仰角
        public float ElevationAngle {
            set { elevationAngle_ = Mathf.Clamp(value, minElevationAngle_, maxElevationAngle_); }
            get { return elevationAngle_; }
        }
        //方位角：現在値,最小値,最大値
        [SerializeField]
        private float azimthAngle_;
        [SerializeField]
        private float minAzimthAngle_ = -120f;
        [SerializeField]
        private float maxAzimthAngle_ = 120f;
	
        //方位角
        public float AzimthAngle {
            set { azimthAngle_ = Mathf.Clamp(value, minAzimthAngle_, maxAzimthAngle_); }
            get { return azimthAngle_; }
        }

        /// <summary>
        /// 初期化
        /// </summary>
		public void Init(float len, float e, float a = 0f) {
            ElevationAngle = e;
            AzimthAngle = a;
            length_ = len;

        }

		public void SetAzimthMinMax(float min, float max) {
			minAzimthAngle_ = min;
			maxAzimthAngle_ = max;
		}
		public void SetElevationMinMax(float min, float max) {
			minElevationAngle_ = min;
			maxElevationAngle_ = max;
		}
		public void SetLengthMinMax(float min, float max, float scl=1f) {
			minLength_ = min;
			maxLength_ = max;
			distanceScale_ = scl;
		}
	}
}
