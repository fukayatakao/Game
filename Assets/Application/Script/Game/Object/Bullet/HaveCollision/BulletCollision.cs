using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 弾が保持するコリジョン
	/// </summary>
	public class BulletCollision : MonoPretender {
		//当たり判定の点
		Transform node_;
		public Transform Node { get { return node_; } }

		//当たり判定が残っているか
		public bool IsAvailable;
        /// <summary>
        /// 初期設定をする
        /// </summary>
		protected override void Create(GameObject obj) {
			node_ = obj.GetComponentInChildren<SphereCollider>().transform;
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public void Init() {
			IsAvailable = true;
		}
	}
}
