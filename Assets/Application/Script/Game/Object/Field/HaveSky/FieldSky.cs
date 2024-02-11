using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// フィールドの天球
	/// </summary>
	public class FieldSky : MonoPretender{
        //天球名
        private string skyName_;
		//アセットのプレハブ
		private Material material_;

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			ResourceCache.UnLoad(skyName_);
			material_ = null;

		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Load(string skyName) {
			//マテリアルをロードする
			skyName_ = skyName;
			material_ = ResourceCache.Load<Material>(skyName);
			//ロードしたマテリアルを天球にセット
			RenderSettings.skybox = material_;
		}
	}
}
