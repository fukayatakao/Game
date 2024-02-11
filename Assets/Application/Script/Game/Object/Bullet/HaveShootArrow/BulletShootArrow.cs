using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 矢の放物運動をする
	/// </summary>
	public class BulletShootArrow : MonoPretender {
		BulletEntity bullet_;
		static readonly Vector3 scale_ = new Vector3(0.01f, 0.01f, 0.01f);
		const float g = 9.8f*10;
		Vector3 moveVector_;
		Vector3 upVector_;
		float groundOffset_;
		/// <summary>
		/// 初期位置を設定
		/// </summary>
		public void Init(BulletEntity bullet, CharacterEntity actor, string nodeName, float groundOffset=0.8f, float additionalLifeTime=1f) {
			bullet_ = bullet;
			groundOffset_ = groundOffset;
			Transform node = actor.HaveCacheNode.SearchNode(nodeName);
			bullet_.SetPosition(node.position);
			bullet_.SetRotation(node.rotation);
			bullet_.SetScale(scale_);
			CharacterEntity target = actor.HaveBlackboard.EnemyBoard.TargetEnemy;


			Vector3 distance;
			if(target == null) {
				distance = actor.GetRotation() * new Vector3(0, 0, 10f);
			} else {
				distance = target.GetPosition() - node.position;
			}
			distance.y = 0f;
			//距離に応じて到達する投射の高さを変えてそれっぽく見せる
			float h = Mathf.Sqrt(distance.magnitude);
			float v0 = Mathf.Sqrt(2 * g * h);
			//重力方向の初速を計算
			upVector_ = new Vector3(0f, v0, 0f);


			//水平方向の初速を計算
			float t = (v0 / g) * 2f;
			moveVector_ = distance / t;
			bullet_.SetLifeTime(t + additionalLifeTime);
		}

		/// <summary>
		/// 移動
		/// </summary>
		public void Execute() {
			if (Time.deltaTime == 0f)
				return;
			if (bullet_ == null)
				return;
			//重力方向+水平方向で移動する
			Vector3 pos = bullet_.GetPosition() + (moveVector_ + upVector_) * Time.deltaTime;
			if (upVector_.y < 0f && pos.y < groundOffset_)
				return;
			//進行方向に向ける
			bullet_.CacheTrans.LookAt(pos);
			bullet_.SetRotation(bullet_.GetRotation() * Quaternion.Euler(90f, 0f, 0f));
			bullet_.SetPosition(pos);

			//重力方向の速度変化
			upVector_.y = upVector_.y - g * Time.deltaTime;

		}

	}
}
