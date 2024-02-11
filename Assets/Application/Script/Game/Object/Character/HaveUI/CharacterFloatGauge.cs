using UnityEngine;
using Project.Lib;
using UnityEngine.UI;

namespace Project.Game {
	/// <summary>
	/// キャラクタのHP/LPゲージ表示
	/// </summary>
	public class CharacterFloatGauge : MonoPretender {
		//前フレームのHP(HP変化検出用)
		[SerializeField]
		private float oldHp_;
		//前フレームのLP(LP変化検出用)
		[SerializeField]
		private float oldLp_;

		//UIが表示されるカメラ
		Camera camera_;
        //ゲージのUI
        Image hpGauge_;
        GameObject hpGaugeObj_;

        Image lpGauge_;
        GameObject lpGaugeObj_;

        //表示位置オフセット
        private const float HP_OFFSET = 0.4f;
        private const float LP_OFFSET = 0.3f;

        //表示時間
        const float DISPLAY_TIME = 3f;
        float lifeTime_ = 0f;

		private bool enable_ = false;
        /// <summary>
        /// インスタンス生成
        /// </summary>
        protected override void Create(GameObject obj) {
        }
        /// <summary>
        /// 破棄
        /// </summary>
        protected override void Destroy() {
	        GameObject.Destroy(hpGaugeObj_);
	        GameObject.Destroy(lpGaugeObj_);

	        enable_ = false;
        }
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Init(Camera camera, Transform canvas) {
            camera_ = camera;


			UnityEngine.U2D.SpriteAtlas atlas = ResourceCache.Load<UnityEngine.U2D.SpriteAtlas>(ResourcesPath.UI_BATTLE_ATLAS);
            GameObject prefab = ResourceCache.Load<GameObject>(ResourcesPath.UI_GAUGE_PREFAB);

			//@note GetSpriteはデータの複製を作るようなのでメモリに注意
			hpGaugeObj_ = UnityUtil.InstantiateChild(canvas, prefab);
			hpGauge_ = hpGaugeObj_.GetComponent<Image>();
            hpGauge_.sprite = atlas.GetSprite("gauge_hp");

            lpGaugeObj_ = UnityUtil.InstantiateChild(canvas, prefab);
            lpGauge_ = lpGaugeObj_.GetComponent<Image>();
            lpGauge_.sprite = atlas.GetSprite("gauge_lp");

			Hide();
			enable_ = true;

		}

		/// <summary>
		/// 初期パラメータを設定
		/// </summary>
		public void SetParam(float iniHp, float iniLp) {
			oldHp_ = iniHp;
			oldLp_ = iniLp;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			if (!enable_)
				return;
			if (!owner.IsAlive)
				return;

			float hp = owner.HaveUnitParam.Fight.Hp;
			float lp = owner.HaveUnitParam.Fight.Lp;
			//HP/LPの変化をポーリング
			if (oldHp_ != hp || oldLp_ != lp) {
				oldHp_ = hp;
				oldLp_ = lp;

				UpdateVariant(hp / owner.HaveUnitParam.Fight.MaxHp, lp / owner.HaveUnitParam.Fight.MaxLp);
			}

			if (lifeTime_ <= 0f)
                return;

            CalcPosition(owner.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head));

            lifeTime_ -= Time.deltaTime;
            if(lifeTime_ <= 0f) {
                Hide();
            }
        }
        /// <summary>
        /// UI表示
        /// </summary>
        public void Show() {
            lifeTime_ = DISPLAY_TIME;
            hpGauge_.enabled = true;
            lpGauge_.enabled = true;
        }
        /// <summary>
        /// UI隠す
        /// </summary>
        public void Hide() {
            lifeTime_ = 0f;
            hpGauge_.enabled = false;
            lpGauge_.enabled = false;
        }
        /// <summary>
        /// パーの状態変化
        /// </summary>
        public void UpdateVariant(float hpRate, float lpRate) {
            hpGauge_.fillAmount = hpRate;
            lpGauge_.fillAmount = lpRate;
            Show();
        }

        /// <summary>
        /// 表示位置を計算する
        /// </summary>
        private void CalcPosition(Transform node) {
            Vector3 pos = node.position;
            float y = pos.y;

            pos.y = y + HP_OFFSET;
            hpGauge_.transform.position = pos;
            hpGauge_.transform.rotation = camera_.transform.rotation;

            pos.y = y + LP_OFFSET;
            lpGauge_.transform.position = pos;
            lpGauge_.transform.rotation = camera_.transform.rotation;
        }
    }
}
