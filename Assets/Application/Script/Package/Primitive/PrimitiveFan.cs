using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// 扇状のプリミティブオブジェクト表示
	/// </summary>
	public class PrimitiveFan : Primitive {
        //背景用のメッシュとマテリアル
        private Mesh mesh_;
        private Vector3[] vertices_;
        //分割数
        const int DivCount = 16;
        /// <summary>
        /// インスタンス生成時処理
        /// </summary>
        public GameObject Create(string resName) {
            mesh_ = CreateMesh(DivCount);
            vertices_ = mesh_.vertices;
            GameObject obj = new GameObject(resName);
            // マテリアルを設定する
            material_ = obj.AddComponent<MeshRenderer>().material;
            SetMaterialSetting(material_);

            obj.GetComponent<MeshRenderer>().material = material_;
            obj.AddComponent<MeshFilter>().mesh = mesh_;

			Init(obj);

			return obj;
        }


		/// <summary>
		/// 扇のパラメータを設定
		/// </summary>
		public void SetFan(float radius, float radian) {
            //頂点情報を設定
            vertices_[0].Set(0f, 0f, 0f);

            float offset = -radian * 0.5f;
            float delta = radian / DivCount;

            //無駄な計算を省いて高速化
            float rad = offset;
            float z = Mathf.Cos(rad) * radius;
            float x = Mathf.Sin(rad) * radius;
            for (int i = 0; i < DivCount; ++i) {
                //前回と同じ頂点になるはず
                vertices_[i * 2 + 1].Set(x, 0f, z);
                //頂点座標を計算
                rad = delta * (i + 1) + offset;
                z = Mathf.Cos(rad) * radius;
                x = Mathf.Sin(rad) * radius;
                vertices_[i * 2 + 2].Set(x, 0f, z);

            }
            mesh_.vertices = vertices_;
			mesh_.RecalculateBounds();
        }

        /// <summary>
        /// 伸縮する用メッシュを生成
        /// </summary>
        private static Mesh CreateMesh(int divCount) {
            Vector3 normal = new Vector3(0f, 1f, 0f);

            int maxVertex = divCount * 2 + 1;
            //頂点座標
            Vector3[] vertexStream = new Vector3[maxVertex];
            //UV座標
            Vector2[] uvStream = new Vector2[maxVertex];
            //法線ベクトル
            Vector3[] normalStream = new Vector3[maxVertex];

            int maxTriangle = divCount * 3;
            //頂点インデックス
            int[] indexStream = new int[maxTriangle];

            Mesh mesh = new Mesh();


            //頂点情報を設定
            uvStream[0].Set(0.5f, 0f);
            normalStream[0] = normal;

            for (int i = 1, max = divCount * 2; i <= max; ++i) {
                uvStream[i].Set((i / (float)divCount), 1f);
                normalStream[i] = normal;

            }

            //三角形ポリゴンの頂点インデックスを設定
            int count = 0;
            for (int i = 0; i < divCount; ++i) {
                indexStream[count] = 0;
                indexStream[count + 1] = i * 2 + 1;
                indexStream[count + 2] = i * 2 + 2;

                count = count + 3;
            }

            //メッシュにデータを渡す
            mesh.vertices = vertexStream;
            mesh.triangles = indexStream;
            mesh.uv = uvStream;
            mesh.normals = normalStream;
            return mesh;
        }


    }
#endif
}
