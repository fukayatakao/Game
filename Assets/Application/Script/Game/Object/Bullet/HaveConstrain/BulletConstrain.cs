using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 弾を指定のノードと同期させる
	/// </summary>
	public class BulletConstrain : MonoPretender {
		BulletEntity entity_;
		Transform node_;
		Transform parent_;
		/// <summary>
		/// 初期設定をする
		/// </summary>
		protected override void Create(GameObject obj) {
			parent_ = obj.transform.parent;
		}
		protected override void Destroy() {
			if(node_ != null) {
				entity_.CacheTrans.SetParent(parent_);
				node_ = null;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="effect"></param>
		public void Init(BulletEntity effect, Transform constrainNode) {
			entity_ = effect;
			// エフェクトと連動する対象のノードを設定
			node_ = constrainNode;

			effect.SetParent(node_);
			//offsetPosition_ = node_.InverseTransformPoint(effect.GetPosition());
			//offsetRotation_ = node_.InverseTransformDirection(effect.GetRotation().eulerAngles);

		}

		public void LateExecute() {
			if (node_ == null)
				return;
			//テスト用に無理やり回転
			//node_.localRotation = node_.localRotation * Quaternion.Euler(0f, 3f, 0f);
			/*entity_.SetPosition(node_.TransformPoint(offsetPosition_));
			entity_.SetRotation(Quaternion.Euler(node_.TransformDirection(offsetRotation_)));*/
		}

	}





}
