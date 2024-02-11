using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// デバッグ用のプリミティブオブジェクト表示
	/// </summary>
	public abstract class Primitive {
		protected Material material_;

		public GameObject gameObject;
		public Transform cacheTrans;



		protected void Init(GameObject obj) {
			gameObject = obj;
			cacheTrans = obj.transform;
		}

		/// <summary>
		/// Active設定
		/// </summary>
		public void SetActive(bool isActive) {
			gameObject.SetActive(isActive);
		}

		/// <summary>
		/// 親になるTransform設定
		/// </summary>
		public void SetParent(Transform parent, bool worldPositionStay = true) {
			gameObject.transform.SetParent(parent, worldPositionStay);
		}
		/// <summary>
		/// 座標設定
		/// </summary>
		public void SetPosition(Vector3 pos) {
			cacheTrans.localPosition = pos;
		}

		/// <summary>
		/// 座標取得
		/// </summary>
		public Vector3 GetPosition() {
			return cacheTrans.localPosition;
		}

		/// <summary>
		/// 回転設定
		/// </summary>
		public void SetRotation(Quaternion rot) {
			cacheTrans.localRotation = rot;
		}
		/// <summary>
		/// 回転取得
		/// </summary>
		public Quaternion GetRotation() {
			return cacheTrans.localRotation;
		}
		/// <summary>
		/// スケール設定
		/// </summary>
		public void SetScale(Vector3 scl) {
			cacheTrans.localScale = scl;
		}
		/// <summary>
		/// スケール取得
		/// </summary>
		public Vector3 GetScale() {
			return cacheTrans.localScale;
		}




		/// <summary>
		/// マテリアルの設定をする
		/// </summary>
		protected void SetMaterialSetting(Material material) {
            material.shader = Shader.Find("Sprites/Default");
		}

		/// <summary>
		/// 色を設定
		/// </summary>
		public void SetColor(Color col) {
            // 色を設定する
            material_.color = col;
		}
		/// <summary>
		/// ワイヤーフレーム表示のon/off
		/// </summary>
		public void SetWireframe(bool flag) {
			if (flag) {
				MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
				meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);

			} else {
				MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
				meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Triangles, 0);
			}
		}
	}
#endif
}
