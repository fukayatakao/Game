using UnityEngine;

namespace Project.Lib {
	public class PrimitivePlane
	{
		/// <summary>
		/// メッシュを生成
		/// </summary>
		public static Mesh Create(float width, float height, int divWidth, int divHeight)
		{
			// Input check
			Debug.Assert(divWidth >= 1, "invalid input divWidth:" + divWidth);
			Debug.Assert(divHeight >= 1, "invalid input divHeight:" + divHeight);

			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;
			Vector3 normal = new Vector3 (0f, 0f, 1f);

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

			float tileWidth = width / divWidth;
			float tileHeight = height / divHeight;



			int count = 0;
			//頂点情報を設定
			for (int j = 0; j <= divHeight; ++j) {
				for (int i = 0; i <= divWidth; ++i) {
					vertexStream [count].Set (-halfWidth + i * tileWidth, halfHeight - j * tileHeight, 0f);
					uvStream [count].Set ((i / (float)divWidth), 1f - (j / (float)divHeight));
					normalStream [count] = normal;

					count++;
				}
			}

			count = 0;
			//三角形ポリゴンの頂点インデックスを設定
			for (int j = 0; j < divHeight; ++j) {
				for (int i = 0; i < divWidth; ++i) {
					if (i >= divWidth / 2) {
						indexStream [count]     = i + (j + 1) * (divWidth + 1);
						indexStream [count + 1] = i + j * (divWidth + 1);
						indexStream [count + 2] = i + 1 + j * (divWidth + 1);

						indexStream [count + 3] = i + (j + 1) * (divWidth + 1);
						indexStream [count + 4] = i + 1 + j * (divWidth + 1);
						indexStream [count + 5] = i + 1 + (j + 1) * (divWidth + 1);
					} else {
                        indexStream [count] = i + 1 + (j + 1) * (divWidth + 1);
                        indexStream [count + 1] = i + (j + 1) * (divWidth + 1);
                        indexStream [count + 2] = i + j * (divWidth + 1);

                        indexStream [count + 3] = i + j * (divWidth + 1);
						indexStream [count + 4] = i + 1 + j * (divWidth + 1);
						indexStream [count + 5] = i + 1 + (j + 1) * (divWidth + 1);
					}
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

        /// <summary>
        /// 伸縮する用メッシュを生成
        /// </summary>
        public static Mesh CreateExpansion(float width, float height, int divWidth, int divHeight) {
            // Input check
            Debug.Assert(divWidth >= 1, "invalid input divWidth:" + divWidth);
            Debug.Assert(divHeight >= 1, "invalid input divHeight:" + divHeight);

            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            Vector3 normal = new Vector3(0f, 0f, 1f);

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

            float tileWidth = width / divWidth;
            float tileHeight = height / divHeight;



            int count = 0;
            //頂点情報を設定
            for (int j = 0; j <= divHeight; ++j) {
                for (int i = 0; i <= divWidth; ++i) {
                    vertexStream[count].Set(-halfWidth + i * tileWidth, halfHeight - j * tileHeight, 0f);
                    uvStream[count].Set((i / (float)divWidth), 1f - (j / (float)divHeight));
                    normalStream[count] = normal;

                    count++;
                }
            }

            count = 0;
            //三角形ポリゴンの頂点インデックスを設定
            for (int j = 0; j < divHeight; ++j) {
                for (int i = 0; i < divWidth; ++i) {
                    indexStream[count] = i + 1 + (j + 1) * (divWidth + 1);
                    indexStream[count + 1] = i + (j + 1) * (divWidth + 1);
                    indexStream[count + 2] = i + j * (divWidth + 1);

                    indexStream[count + 3] = i + j * (divWidth + 1);
                    indexStream[count + 4] = i + 1 + j * (divWidth + 1);
                    indexStream[count + 5] = i + 1 + (j + 1) * (divWidth + 1);
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
