using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 建物が保持するコリジョン
	/// </summary>
	public class FacilityCollision : MonoPretender {
		//建物のコリジョン半径。リンクの矢印が埋もれないようにずらすのに使う。
		float radius_;
		public float GetRadius() { return radius_; }

		private Vector3 size_;
		public Vector3 GetSize() { return size_; }

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);

			//コリジョンの長辺の半分を半径として使う
			var collider = obj.GetComponent<BoxCollider>();
			Debug.Assert(collider != null, "not found collider");
			size_ = collider.size;
			radius_ = (size_.x > size_.z ? size_.x : size_.z) * 0.5f;

		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			base.Destroy();
		}

	}
}
