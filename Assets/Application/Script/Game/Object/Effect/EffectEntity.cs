using Project.Lib;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// エフェクト
	/// </summary>
	public class EffectEntity : Entity {

		private float timer_;
		private float lifeTime_;

		public bool IsAutoDestroy { get; set; }
		GameObject asset_;

		private EffectComponent haveComponent_;

		//特定のノードに固定する
		private EffectConstrain haveConstrain_;
		public EffectConstrain HaveConstrain { get { return haveConstrain_; } }

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string name) {
			asset_ = (GameObject)await AddressableAssist.LoadAssetAsync(name);
			GameObject obj = UnityUtil.Instantiate(asset_);
			obj.name = name;
			//必要なデータを保持する
			haveComponent_ = new EffectComponent(obj);

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
			lifeTime_ = float.MaxValue;
			timer_ = 0f;
			for (int i = 0, max = haveComponent_.DollyCarts.Length; i < max; i++) {
				haveComponent_.DollyCarts[i].m_Position = 0f;
			}

			IsAutoDestroy = true;
		}
		/// <summary>
		/// テンポラリの初期化
		/// </summary>
		public override void Cleanup() {
			MonoPretender.Destroy(haveConstrain_);
		}
		/// <summary>
		/// エフェクトの位置をノードに固定する
		/// </summary>
		public void Constrain(Transform node) {
			haveConstrain_ = MonoPretender.Create<EffectConstrain>(gameObject_);
			haveConstrain_.Init(this, node);
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
			if (timer_ == float.MaxValue)
				return;

			timer_ += Time.deltaTime;
			if ( lifeTime_ <= timer_ ) {
				Stop();
			}
		}
		/// <summary>
		/// 実行(後)処理
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

			//ストップがかかっていたらAnimationは終了しているとみなす
			if (timer_ != float.MaxValue) {
				for (int i = 0, max = haveComponent_.Animators.Length; i < max; i++) {
					if (haveComponent_.Animators[i].GetCurrentAnimatorStateInfo(0).loop)
						return true;
					if (haveComponent_.Animators[i].GetCurrentAnimatorStateInfo(0).length >= timer_) {
						return true;
					}
				}
				for (int i = 0, max = haveComponent_.DollyCarts.Length; i < max; i++) {
					var cart = haveComponent_.DollyCarts[i];
					switch (cart.m_PositionUnits) {
						case CinemachinePathBase.PositionUnits.Normalized:
							if (haveComponent_.DollyCarts[i].m_Position < 1f)
								return true;
							break;
						default:
							//正規化されたカメラパス以外は未サポート
							Debug.LogError("not support type:" + cart.m_PositionUnits);
							break;
					}
				}
			}

			//再生停止後もパーティクルが残っている間は残す
			for(int i = 0, max = haveComponent_.Particles.Length; i < max; i++) {
				ParticleSystem p = haveComponent_.Particles[i];
				if( !p )
					continue;
				//ループエフェクトの場合はストップされない限り再生中とみなす
				if (p.main.loop && timer_ != float.MaxValue)
					return true;
				//@note エミッタ止めてもduration分の時間は生き続ける（毎回IsAliveするよりはちょっと軽くなるはず
				// p.IsAvailable()が重いので最後に判定
				if (p.main.duration >= timer_ || p.IsAlive(false)) {
					return  true;
				}
			}

			return false;
		}
		/// <summary>
		/// エフェクトを止める
		/// </summary>
		public void Stop() {
			Resume();
			timer_ = float.MaxValue;
			for (int i = 0, max = haveComponent_.Particles.Length; i < max; i++) {
				ParticleSystem p = haveComponent_.Particles[i];
				//エミッターだけ止めて既存のパーティクルは残す
				p.Stop();
			}
		}

		/// <summary>
		/// エフェクトを一時停止
		/// </summary>
		public void Suspend()
		{
			for(int i = 0, max = haveComponent_.Animators.Length; i < max; i++)
			{
				haveComponent_.Animators[i].speed = 0;
			}
			for (int i = 0, max = haveComponent_.Particles.Length; i < max; i++)
			{
				ParticleSystem p = haveComponent_.Particles[i];
				p.Pause();
			}
		}
		/// <summary>
		/// エフェクトを一時停止再開
		/// </summary>
		public void Resume()
		{
			for(int i = 0, max = haveComponent_.Animators.Length; i < max; i++)
			{
				haveComponent_.Animators[i].speed = 1.0f;
			}
			for (int i = 0, max = haveComponent_.Particles.Length; i < max; i++)
			{
				ParticleSystem p = haveComponent_.Particles[i];
				p.Play();
			}
		}

	}





}
