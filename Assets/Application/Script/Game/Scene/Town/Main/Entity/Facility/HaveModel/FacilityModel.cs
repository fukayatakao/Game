using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 建物が保持するコリジョン
	/// </summary>
	public class FacilityModel : MonoPretender {
		GameObject model_;
		Outline outline_;
		Material material_;
		Color defaultColor_;

		public void Select() {
			model_.SetActive(true);
			outline_.enabled = true;
		}
		public void UnSelect() {
			model_.SetActive(false);
			outline_.enabled = false;
		}
		public void SetBuildable(bool flag) {
			if (!flag) {
				material_.color = Color.red;
			} else {
				material_.color = defaultColor_;
			}
		}
		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);

			SetupPrimitiveModel(obj);

		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			base.Destroy();
		}

		/// <summary>
		/// 建物を覆うプリミティブな立方体
		/// </summary>
		private void SetupPrimitiveModel(GameObject root) {
			var debugFacility = root.GetComponent<AttachDebugFacility>();

			model_ = GameObject.CreatePrimitive(PrimitiveType.Cube);
			model_.transform.SetParent(transform);
			model_.transform.localPosition = new Vector3(0f, debugFacility.size_.y * 0.5f + 0.001f, 0f);
			model_.transform.localScale = debugFacility.size_;
			model_.name = "Model";
			model_.SetActive(false);

			//マウス操作を受け取るコリジョンは立方体ではなくprefabのrootにつける。model側のコリジョンは削除。
			GameObject.Destroy(model_.GetComponent<BoxCollider>());
			gameObject.AddComponent<BoxCollider>().size = debugFacility.size_;
			UnityUtil.SetLayer(transform, (int)UnityLayer.Layer.Facility);

			InitModel(model_, debugFacility);
			InitLabel(gameObject, debugFacility);

		}
		/// <summary>
		/// 仮モデルを作る
		/// </summary>
		private void InitModel(GameObject node, AttachDebugFacility debugFacility) {
			//透過前提でシェーダを選択する
			material_ = node.GetComponent<Renderer>().material;
			material_.shader = Shader.Find("Sprites/Default");
			material_.color = debugFacility.color_;
			defaultColor_ = material_.color;
			node.GetComponent<MeshRenderer>().material = material_;
			outline_ = node.AddComponent<Outline>();
			outline_.OutlineWidth = 6f;
			outline_.enabled = false;
		}

		/// <summary>
		/// 建物の種別名を表示するテキストを設定
		/// </summary>
		private void InitLabel(GameObject node, AttachDebugFacility debugFacility) {
			//フォントをセットしてあるプレハブアセットをロードしてインスタンス作成
			GameObject obj = ResourceCache.Load<GameObject>(ResourcesPath.UI_HEAD_TEXT);
			GameObject textObj = UnityUtil.Instantiate(obj);
			textObj.transform.SetParent(node.transform);
			textObj.transform.localPosition = new Vector3(0f, debugFacility.size_.y + 5f, 0f);
			//テキストメッシュの内容を設定
			var textMesh = textObj.GetComponent<TMPro.TextMeshPro>();
			textMesh.text = debugFacility.text_;
			textMesh.fontSize = debugFacility.fontSize_;

		}
	}
}
