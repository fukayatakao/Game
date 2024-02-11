using UnityEngine;

namespace Project.Lib {
	public class PrimitiveCurveMesh {
		GameObject model_;
		//背景用のメッシュとマテリアル
		private Mesh mesh_;
		private Vector3[] vertices_;
		MeshCollider collider_;
		MeshRenderer renderer_;
		//縦横分割数
		const int SubdivionsWidth = 128;
		const int SubdivionsHeight = 1;


		/// <summary>
		/// 初期化
		/// </summary>
		public GameObject Create(GameObject asset) {
			model_ = UnityUtil.Instantiate(asset);
			mesh_ = CreateMesh(SubdivionsWidth, SubdivionsHeight);
			vertices_ = mesh_.vertices;

			renderer_ = model_.GetComponent<MeshRenderer>();
			MeshFilter meshFilter = model_.GetComponent<MeshFilter>();
			MeshCollider collider = model_.GetComponent<MeshCollider>();
			meshFilter.mesh = mesh_;
			collider_ = collider;

			return model_;
		}

		//@note GameObject自体をon/offするとコリジョンがうまく反応しないのでrendererだけ消す
		/// <summary>
		/// 表示設定
		/// </summary>
		public void SetVisible(bool isActive) {
			renderer_.enabled = isActive;
		}

		/// <summary>
		/// 親になるTransform設定
		/// </summary>
		public void SetParent(Transform parent, bool worldPositionStay = true) {
			model_.transform.SetParent(parent, worldPositionStay);
		}



		/// <summary>
		/// 曲線計算
		/// </summary>
		public void Calculate(System.Func<float, float> evaluate, float width, float height, float y) {
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;
			float tileWidth = width / SubdivionsWidth;
			float tileHeight = height / SubdivionsHeight;
			int count = 0;

			Vector3 current = Vector3.zero;
			Vector3 next = Vector3.zero;
			Vector3 prev = Vector3.zero;

			//頂点情報を設定
			for (int i = 0; i <= SubdivionsWidth; ++i) {
				//最初の点は計算する
				if (i == 0) {
					current.x = -halfWidth + i * tileWidth;
					current.z = evaluate(current.x);
					//２つ目以降は前回計算したnextの値を使う
				} else {
					current = next;
				}
				Quaternion quat = Quaternion.identity;

				bool half = false;
				if (i - 1 >= 0) {
					quat = MathUtil.LookAtY(current - prev);
					half = true;
				}
				if (i + 1 <= SubdivionsWidth) {
					next.x = -halfWidth + (i + 1) * tileWidth;
					next.z = evaluate(next.x);
					Quaternion q2 = MathUtil.LookAtY(next - current);
					if (half) {
						quat = Quaternion.Slerp(quat, q2, 0.5f);
					} else {
						quat = q2;
					}
				}
				for (int j = 0; j <= SubdivionsHeight; ++j) {
					Vector3 vec = quat * new Vector3(-1f, 0f, 0f) * (halfHeight - j * tileHeight);
					vertices_[count].Set(current.x + vec.x, y, current.z + vec.z);
					count++;
				}

				prev = current;
			}
			mesh_.vertices = vertices_;
			mesh_.RecalculateBounds();
			if(collider_ != null)
				collider_.sharedMesh = mesh_;

		}

		/// <summary>
		/// 伸縮する用メッシュを生成
		/// </summary>
		private static Mesh CreateMesh(int divWidth, int divHeight) {
			// Input check
			Debug.Assert(divWidth >= 1, "invalid input divWidth:" + divWidth);
			Debug.Assert(divHeight >= 1, "invalid input divHeight:" + divHeight);

			Vector3 normal = new Vector3(0f, 1f, 0f);

			int maxVertex = (divHeight + 1) * (divWidth + 1);
			//頂点座標
			Vector3[] vertexStream = new Vector3[maxVertex];
			//UV座標
			Vector2[] uvStream = new Vector2[maxVertex];
			//法線ベクトル
			Vector3[] normalStream = new Vector3[maxVertex];

			int maxTriangle = divHeight * divWidth * 2 * 3;
			//頂点インデックス
			int[] indexStream = new int[maxTriangle];

			Mesh mesh = new Mesh();


			int count = 0;
			//頂点情報を設定
			for (int i = 0; i <= divWidth; ++i) {
				for (int j = 0; j <= divHeight; ++j) {
					//vertexStream[count].Set(-halfWidth + i * tileWidth, 0f, halfHeight - j * tileHeight);
					uvStream[count].Set((i / (float)divWidth), 1f - (j / (float)divHeight));
					normalStream[count] = normal;

					count++;
				}
			}

			count = 0;
			//三角形ポリゴンの頂点インデックスを設定
			for (int i = 0; i < divWidth; ++i) {
				for (int j = 0; j < divHeight; ++j) {
					int offset = divHeight + 1;

					indexStream[count] = (i + 1) * offset + (j + 1);
					indexStream[count + 1] = i * offset + (j + 1);
					indexStream[count + 2] = i * offset + j;

					indexStream[count + 3] = i * offset + j;
					indexStream[count + 4] = (i + 1) * offset + j;
					indexStream[count + 5] = (i + 1) * offset + (j + 1);
					count = count + 6;
				}
			}

			//メッシュにデータを渡す
			mesh.vertices = vertexStream;
			mesh.triangles = indexStream;
			mesh.uv = uvStream;
			mesh.normals = normalStream;
			return mesh;
		}


	}
}
