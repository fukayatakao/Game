using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// フィールドのステージ
	/// </summary>
	public class FieldStage {
		//親のゲームオブジェクト
		private Transform parent_;
		//アセットのプレハブ
		private GameObject prefab_;
		//ステージ
		private GameObject object_;

		private FieldLightMap lightMap_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FieldStage(Transform parent) {
			parent_ = parent;
		}
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(string stageName) {
			Load (stageName);
			//ロードに失敗してたらAssert
			Debug.Assert (prefab_ != null, "Failed to load field");

			object_ = UnityUtil.InstantiateChild(parent_, prefab_);
			lightMap_ = object_.GetComponent<FieldLightMap> ();

			if(lightMap_ != null)
				lightMap_.Setup ();

			//UtilManager.SetLayer (object_.transform, (int)GardenMain.SceneLayer.Field);
		}
		/// <summary>
		/// クリーンアップ
		/// </summary>
		public void Cleanup() {
			if(lightMap_ != null)
				lightMap_.Cleanup ();
			//インスタンス破棄
			GameObject.Destroy (object_);
			object_ = null;
			//リソース破棄
			UnLoad ();
		}
		/// <summary>
		/// リソース読み込んでメモリに置く
		/// </summary>
		private void Load (string stageName) {
            prefab_ = ResourceCache.Load<GameObject>(stageName);
        }
		/// <summary>
		/// リソース読み込んでメモリに置く
		/// </summary>
		private void UnLoad () {
			//ResourcesAsset.Field.Stage.UnLoad ();
		}
        // <summary>
		/// ライトマップを反映
		/// </summary>
		public void ApplyLightMap(){
			FieldLightMap map = object_.GetComponent<FieldLightMap> ();
			if (map != null)
				map.Setup();
		}




    }
}
