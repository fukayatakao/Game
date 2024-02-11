using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// エフェクトを指定のノードと同期させる
	/// </summary>
	public class EffectConstrain : MonoPretender {
		//座標を同期
		private bool enablePosition_;
		private Vector3 offsetPosition_;
		//回転を同期
		private bool enableRotation_;
		private Quaternion offsetRotation_;
		//スケールを同期
		private bool enableScale_;
		private Vector3 offsetScale_;


		EffectEntity entity_;
		Transform node_;

		float durationTime_;
		float timer_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="effect"></param>
		public void Init(EffectEntity effect, Transform constrainNode) {
			entity_ = effect;

			// エフェクトと連動する対象のノードを設定
			node_ = constrainNode;
			durationTime_ = float.MaxValue;
			timer_ = 0f;
			enablePosition_ = false;
			enableRotation_ = false;
		}

		/// <summary>
		/// 連動する期間を設定
		/// </summary>
		/// <param name="duration"></param>
		public void SetDuration(float duration) {
			durationTime_ = duration;
			timer_ = 0f;
		}

		/// <summary>
		/// 座標を連動
		/// </summary>
		public void ConstrainPosition(Vector3 pos) {
			enablePosition_ = true;
			offsetPosition_ = pos;
			entity_.SetPosition(node_.TransformPoint(offsetPosition_));
		}
		/// <summary>
		/// 回転を連動
		/// </summary>
		public void ConstrainRotation(Quaternion rot) {
			enableRotation_ = true;
			offsetRotation_ = rot;
			entity_.SetRotation(node_.rotation * offsetRotation_);
		}
		/// <summary>
		/// サイズを連動
		/// </summary>
		public void ConstrainScale(Vector3 scl) {
			enableScale_ = true;
			offsetScale_ = scl;
			entity_.SetScale(new Vector3(node_.lossyScale.x * offsetScale_.x, node_.lossyScale.y * offsetScale_.y, node_.lossyScale.z * offsetScale_.z));
		}


		public void LateExecute() {
			if (node_ == null)
				return;
			//時間0で１回は実行したいので=は含めないの
			if (timer_ > durationTime_)
				return;

			if (enablePosition_) {
				entity_.SetPosition(node_.TransformPoint(offsetPosition_));
			}
			if (enableRotation_) {
				entity_.SetRotation(node_.rotation * offsetRotation_);
			}
			if (enableScale_) {
				entity_.SetScale(new Vector3(node_.lossyScale.x * offsetScale_.x, node_.lossyScale.y * offsetScale_.y, node_.lossyScale.z * offsetScale_.z));
			}
			timer_ += Time.deltaTime;
		}

	}





}
