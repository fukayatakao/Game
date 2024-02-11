using UnityEngine;

namespace Project.Lib {
	 /// <summary>
    /// ぷニコン的な操作（最初にタッチした位置を基点にして～
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class TouchController : MonoBehaviour{
        //何番目にタッチした指で操作するか
        const int CONTROL_INDEX = 0;
        private const float TargetMin = 0.1f;
        private const float TargetMax = 1.0f;
        //基点からの距離(操作感度で補正した値TargetMin～TargetMax
        protected float targetY_;
        public float TargetY { get { return targetY_; } }

        //基点からの方向(Y軸上をデフォルト
        protected Quaternion rotationXZ_ = new Quaternion();
        public Quaternion RotationXZ { get { return rotationXZ_; } }

        //基点座標
        protected Vector3 basePos_;
        //@note このサイズを大きくすると大きくスワイプしないといけなくなる
        //操作感度1.0とみなすスワイプ距離
        protected float length_ = 1;
        /// <summary>
        /// <summary>
        /// 実行処理
        /// </summary>
        private void Update() {

            GestureControll(CONTROL_INDEX);

            // 最小・最大量にまるめる
            if (targetY_ < TargetMin)
                targetY_ = 0f;
            if (targetY_ > TargetMax)
                targetY_ = TargetMax;
        }

        protected void GestureControll(int index) {
            //最初にタッチしたときに基点を決定
            if (Project.Lib.Gesture.IsTouchDown(index)) {
                basePos_ = VirtualScreen.PixelToScreenPos(Project.Lib.Gesture.GetTouchPos(index));
            //タッチ離したらリセット
            } else if (Project.Lib.Gesture.IsTouchUp(index)) {
                targetY_ = 0f;
            //スワイプ中なら変化量と方向を計算
            } else if (Project.Lib.Gesture.IsSwipe(index)) {
                Calculate(VirtualScreen.PixelToScreenDelta(Project.Lib.Gesture.GetSwipeDirect(index)));
            }
        }
        //半角sin,cos
        static readonly float qz90 = Mathf.Sin(-45f * Mathf.Deg2Rad);
        static readonly float qw90 = Mathf.Cos(-45f * Mathf.Deg2Rad);


        /// <summary>
        /// タッチ座標からプニコンのパラメータを設定
        /// </summary>
        protected void Calculate(Vector2 delta) {
            float x = delta.x;
            float y = delta.y;
            float length = Mathf.Sqrt(x * x + y * y);
            targetY_ = length / length_;

            //xを正規化=cosθと同値
            float xx = x / length;
            //半角定理を使ってcosθからsin(θ/2),cos(θ/2)を導出、クォータニオンを直接作る
            float qz = Mathf.Sqrt((1f - xx) * 0.5f) * Mathf.Sign(y);
            float qw = Mathf.Sqrt((1f + xx) * 0.5f);

            //Z軸回転をY軸回転に置き換えて保存する
            rotationXZ_.y = -(qz * qw90 + qw * qz90);
            rotationXZ_.w = (qw * qw90 - qz * qz90);
        }
    }
}
