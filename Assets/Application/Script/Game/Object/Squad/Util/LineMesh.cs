using Project.Lib;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 曲線表示メッシュ
	/// </summary>
	public class LineMesh {
		UnityEngine.Object asset_;
		PrimitiveCurveMesh primitive_ = new PrimitiveCurveMesh();

		System.Func<float, float> evaluate_;
        //縦横サイズ
        [SerializeField]
        private float width_ = 10f;
        [SerializeField]
        private float height_ = 0.5f;
		public float Height { get { return height_; } set { height_ = value; } }
		[SerializeField]
        private float offsetY_ = 0.01f;


		/// <summary>
		/// アセットロード
		/// </summary>
		public async Task LoadAsync(string assetName) {
			asset_ = await AddressableAssist.LoadAssetAsync(assetName);
		}

		/// <summary>
		/// アセット破棄
		/// </summary>
		public void UnLoad() {
			AddressableAssist.UnLoadAsset(asset_);
		}

		/// <summary>
		/// 3Dモデル生成
		/// </summary>
		public GameObject Create(Transform parent) {
			GameObject obj = primitive_.Create((GameObject)asset_);
			primitive_.SetParent(parent);
			return obj;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(System.Func<float, float> evaluate, float width, float height, float y) {
			width_ = width;
            height_ = height;
            offsetY_ = y;
			evaluate_ = evaluate;
		}

		/// <summary>
		/// 曲線計算
		/// </summary>
		public void Calculate() {
			primitive_.Calculate(evaluate_, width_, height_, offsetY_);
        }

		public void Show() {
			primitive_.SetVisible(true);
		}

		public void Hide() {
			primitive_.SetVisible(false);
		}
	}
#endif
}
