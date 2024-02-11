using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// 基本的なプリミティブオブジェクト表示
	/// </summary>
	public abstract class PrimitiveStandard : Primitive {
		protected abstract PrimitiveType primitiveType { get; }

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public GameObject Create(string resName) {
            GameObject obj = GameObject.CreatePrimitive(primitiveType);
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

	/// <summary>
	/// 球プリミティブ
	/// </summary>
	public class PrimitiveSphere : PrimitiveStandard {
		protected override PrimitiveType primitiveType { get { return PrimitiveType.Sphere; } }
	}
	/// <summary>
	/// 立方体プリミティブ
	/// </summary>
	public class PrimitiveCube : PrimitiveStandard {
		protected override PrimitiveType primitiveType { get { return PrimitiveType.Cube; } }
	}
#endif
}
