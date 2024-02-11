using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// 円状エリア
	/// </summary>
	public class CircleArea : MonoPretender {
		static readonly Quaternion INIT_QUAT = Quaternion.Euler(90f, 0f, 0f);
		static readonly Vector3 INIT_POS = new Vector3(0f, 0.01f, 0f);
		//オブジェクトのキャッシュ
		private Transform cachTrans_;
		//オブジェクトで使っているマテリアルのキャッシュ
		Material material_;

		//設定可能な最大半径
		[SerializeField]
		float maxRadius_;
		public float MaxRadius{ get{ return maxRadius_; } }

		//半径
		[SerializeField]
		float radius_;
		public float Radius { get { return radius_; } }

		//半径の2乗値
		float radiusSq_;
		public float RadiusSq { get { return radiusSq_; } }

		//半径の2乗値
		float radiusExSq_;
		public float RadiusExSq { get { return radiusExSq_; } }

		[SerializeField]
		Color color_;

		Object circle_;

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
			//ちゃんと破棄がされてないんじゃないか疑惑をチェック
			Debug.Assert(circle_ == null, "circle instance is not null");
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			AddressableAssist.UnLoadAsset(circle_);
			circle_ = null;
			base.Destroy();
		}
		/// <summary>
		/// アセットロード
		/// </summary>
		public async Task LoadAsync() {
			circle_ = await AddressableAssist.LoadAssetAsync(AddressableDefine.Address.CIRCLE_AREA);
		}
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(Transform parent, float radius, Color col) {
			//インスタンスの生成とキャッシュ
			GameObject obj = UnityUtil.InstantiateChild(parent, circle_ as GameObject);
			material_ = obj.GetComponent<MeshRenderer>().material;
			cachTrans_ = obj.transform;

			//固定値で初期化
			cachTrans_.localRotation = INIT_QUAT;
			cachTrans_.localPosition = INIT_POS;
			//設定可能な最大半径
			maxRadius_ = radius;
			//可変値で初期化
			SetRadius(radius);
			SetColor(col);
			SetWidth(1f / radius);

			SetVisible(false);
		}
		/// <summary>
		/// 表示制御
		/// </summary>
		public void SetVisible(bool active) {
			cachTrans_.gameObject.SetActive(active);
		}
		/// <summary>
		/// 半径をセット
		/// </summary>
		public void SetRadius(float r) {
			radius_ = r;
			radiusSq_ = radius_ * radius_;
			radiusExSq_ = (radius_ + GameConst.Town.EXTEND_AREA_RADIUS) * (radius_ + GameConst.Town.EXTEND_AREA_RADIUS);
			cachTrans_.localScale = new Vector3(radius_ * 2f, radius_ * 2f, 1f);
		}
		/// <summary>
		/// 円のカラーをセット
		/// </summary>
		private void SetColor(Color col) {
			color_ = col;
			material_.SetColor("_Color", color_);
		}
		/// <summary>
		/// 線の幅をセット
		/// </summary>
		private void SetWidth(float w) {
			material_.SetFloat("_Width", w);
		}

#if USE_MONOBEHAVIOUR
		/// <summary>
		/// インスペクタの値に変化があった
		/// </summary>
		void OnValidate() {
			if (cachTrans_ == null)
				return;
			SetRadius(radius_);
			SetColor(color_);
		}
#endif
	}
}
