using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// キャラクターが保持するコリジョン
	/// </summary>
	public class BodyCollision : MonoPretender {
		[SerializeField]
        private float radius_;
        public float Radius{get{ return radius_; } }

		//コリジョンの押出補正値
		public Vector3 Correct = Vector3.zero;
		CapsuleCollider capsule_;
		public CapsuleCollider Capsule { get { return capsule_; } }

		/// <summary>
		/// 初期設定をする
		/// </summary>
		protected override void Create(GameObject obj) {
			capsule_ = obj.GetComponentInChildren<CapsuleCollider>();
			if(capsule_ == null) {
				capsule_ = obj.AddComponent<CapsuleCollider>();
				capsule_.radius = 0.5f;
				capsule_.height = 1.5f;
				capsule_.center = new Vector3(0f, 0.25f, 0f);

			}
			capsule_.isTrigger = true;

			Init(obj.transform.localScale.x);

		}
        /// <summary>
        /// 初期化
        /// </summary>
        private void Init(float scale = 1f) {
			radius_ = capsule_.radius * scale;

        }

    }
}
