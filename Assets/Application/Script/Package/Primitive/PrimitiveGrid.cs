using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// グリッドマップ
	/// </summary>
	public class PrimitiveGrid : Primitive {
		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public GameObject Create(string resName) {
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
			obj.name = resName;

			//CreatePrimitiveで作るとColliderが付いてくるけど要らないので削除
			GameObject.Destroy(obj.GetComponent<Collider>());
			// マテリアルを設定する
			material_ = obj.GetComponent<Renderer>().material;
			SetMaterialSetting(material_);

			Init(obj);

			return obj;
		}
	}

#endif
}
