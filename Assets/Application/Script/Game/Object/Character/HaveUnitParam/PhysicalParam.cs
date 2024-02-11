using UnityEngine;


namespace Project.Game {
    /// <summary>
    /// キャラクターの身体能力
    /// </summary>
	[System.Serializable]
    public class PhysicalParam {

		//空間要素
		//移動速度(m/s)
		public float MaxSpeed;
        //加速度(m/s)
        public float Acceleration;

        //現在速度
        [SerializeField]
        private float currentSpeed_;
        public float CurrentSpeed {
            get { return currentSpeed_; }
            set {
                currentSpeed_ = value;
                if (currentSpeed_ < 0f) currentSpeed_ = 0f;
                else if (currentSpeed_ > MaxSpeed) currentSpeed_ = MaxSpeed;
            }
        }
		/// <summary>
		/// 加速処理
		/// </summary>
		public void Accelerate() {
			CurrentSpeed = currentSpeed_ + Acceleration * Time.deltaTime;
		}
		/// <summary>
		/// 減速処理
		/// </summary>
		public void Deaccelerate() {
			CurrentSpeed = currentSpeed_ - Acceleration * Time.deltaTime;
		}

		//回転速度(rad/s)
		public float AimSpeedDeg;
		public float AimSpeedRad { get { return AimSpeedDeg * Mathf.Deg2Rad; } }
		//索敵距離(m)
		public float SearchRange;

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(Mst.MstUnitData master) {
			//移動速度・加速度
			MaxSpeed = master.MaxSpeed;
			Acceleration = master.Accelerate;

			//攻撃可能前方範囲
			AimSpeedDeg = master.AimSpeedDeg;

			//索敵距離
			SearchRange = master.SearchRange;
		}
		/// <summary>
		/// 制動距離を計算
		/// </summary>
		public float CalcSlipDistance(float speed) {
            float t = (speed - GameConst.Battle.STATIC_FRICTION) / Acceleration;
            if (t < 0)
                return 0f;
            return speed * t - 0.5f * Acceleration * t * t;
        }

    }
}
