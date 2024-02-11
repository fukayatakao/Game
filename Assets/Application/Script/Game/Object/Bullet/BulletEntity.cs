using Project.Lib;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 弾
	/// </summary>
	public class BulletEntity : Entity {
		private float timer_;
		private float lifeTime_;

		public bool IsAutoDestroy { get; set; }
		GameObject asset_;

		private BulletCollision haveCollision_;		
		public BulletCollision HaveCollision { get { return haveCollision_; } }

		//矢の軌道で移動する
		private BulletShootArrow haveShootArrow_;
		public BulletShootArrow HaveShootArrow { get { return haveShootArrow_; } }

		private BulletConstrain haveConstrain_;
		public BulletConstrain HaveConstrain { get { return haveConstrain_; } }

		public ActionDumpData ActionDump;

		const string EFFECT_PATH = "Effect/";

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string name) {
			asset_ = (GameObject)await AddressableAssist.LoadAssetAsync(EFFECT_PATH + name);
			GameObject obj = UnityUtil.Instantiate(asset_);
			obj.name = name;


			haveCollision_ = MonoPretender.Create<BulletCollision>(obj);

			return obj;
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			AddressableAssist.UnLoadAsset(asset_);
			asset_ = null;
		}

		/// <summary>
		/// テンポラリの初期化
		/// </summary>
		public override void Setup(){
			//コリジョンを有効化する
			haveCollision_.Init();
			lifeTime_ = float.MaxValue;
			timer_ = 0f;

			IsAutoDestroy = true;
		}
		/// <summary>
		/// テンポラリの初期化
		/// </summary>
		public override void Cleanup() {
			MonoPretender.Destroy(haveCollision_);
			MonoPretender.Destroy(haveShootArrow_);
			MonoPretender.Destroy(haveConstrain_);
		}
		/// <summary>
		/// 弾を発射する
		/// </summary>
		public void Shoot(ActionDumpData dump, string nodeName, float groundOffset = 0.8f, float additionalLifeTime = 1f) {
			ActionDump = dump;
			haveShootArrow_ = MonoPretender.Create<BulletShootArrow>(gameObject_);
			haveShootArrow_.Init(this, ActionDump.Actor, nodeName, groundOffset, additionalLifeTime);
			IsAutoDestroy = true;
		}
		/// <summary>
		/// 当たった
		/// </summary>
		public void Hit(CharacterEntity entity) {
			haveConstrain_ = MonoPretender.Create<BulletConstrain>(gameObject_);
			haveConstrain_.Init(this, entity.CacheTrans);
			MonoPretender.Destroy(haveShootArrow_);
		}

		public bool IsAlive() {
			return haveCollision_.IsAvailable;
		}

		/// <summary>
		/// 再生時間設定
		/// </summary>
		public void SetLifeTime(float time) {
			lifeTime_ = time;
		}
		/// <summary>
		/// 実行処理
		/// </summary>
		public override void Execute(){
			if (haveShootArrow_ != null) haveShootArrow_.Execute();

			timer_ += Time.deltaTime;
		}
		/// <summary>
		/// 実行処理
		/// </summary>
		public override void LateExecute() {
			if (haveConstrain_ != null) haveConstrain_.LateExecute();
		}
		/// <summary>
		/// 再生中か
		/// </summary>
		public override bool IsExist() {
			if (!IsAutoDestroy)
				return true;
			return lifeTime_ > timer_; 
		}
	}





}
