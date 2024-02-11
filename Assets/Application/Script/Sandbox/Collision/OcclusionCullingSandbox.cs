using UnityEngine;
using System.Collections.Generic;

using Project.Game;
using System.Threading;

namespace Project.Lib {
#if UNITY_EDITOR && DEVELOP_BUILD
	/// <summary>
	/// コリジョンのデバッグ表示
	/// </summary>
	public class OcclusionCullingSandbox : MonoBehaviour {
		//Entity集合クラス
		AssemlyManager assemblyManager_ = new AssemlyManager(
			new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
				CameraAssembly.CreateInstance,
		   }
		);

		CameraEntity mainCamera_;
		//ビュー行列をカメラの値から更新するか
		bool isUpdateMatrix = true;
		Matrix4x4 matrix = Matrix4x4.identity;

		const int OccluderMax = 10;

		//カリングの対象オブジェクト
		class OccluerObject {
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public OccluerObject(Transform parent) {
				occluder = new Occluder();
				quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
				quad.transform.parent = parent;
				GameObject.Destroy(quad.GetComponent<Collider>());
			}
			public Occluder occluder;
			//表示用quad
			public GameObject quad;
		}
		OccluerObject[] occluderObject = new OccluerObject[OccluderMax];
		Color[] occluderColor = new Color[OccluderMax]
		{
			new Color(0.3f, 0f, 0f),
			new Color(0.6f, 0f, 0f),
			new Color(0f, 0.3f, 0f),
			new Color(0f, 0.6f, 0f),
			new Color(0f, 0f, 0.3f),
			new Color(0f, 0f, 0.6f),
			new Color(0.3f, 0.3f, 0f),
			new Color(0.3f, 0.6f, 0f),
			new Color(0.6f, 0.6f, 0f),
			new Color(0.6f, 0.6f, 0.6f),
		};


		const int ObjectMax = 1000;

		//カリングの対象オブジェクト
		class CullingObject {
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public CullingObject(Transform parent) {
				obb = new ShapeCollision.OBB();
				cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.parent = parent;
				GameObject.Destroy(cube.GetComponent<Collider>());
			}
			//境界Box
			public ShapeCollision.OBB obb;
			//隣接球の半径
			public float radius;

			//表示用cube
			public GameObject cube;
			//カリング対象か
			public bool isCulling;
		}

		CullingObject[] cullingTarget;


		OcclusionCulling culling = new OcclusionCulling();
		async void Awake(){
			Gesture.Create();
			VirtualScreen.Create();
			cullingTarget = new CullingObject[ObjectMax];

			EditorViewControl.TargetMoveRatio = 0.1f;

			assemblyManager_.Create(transform);

			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.VIEW_CAMERA_PREFAB, true);
			mainCamera_.EditorView.Begin();
			mainCamera_.ChangeControlEditorView(true);

			Transform occluderParent = new GameObject("occluder").transform;
			//遮蔽板を生成
			for (int i = 0; i < OccluderMax; i++) {
				occluderObject[i] = new OccluerObject(occluderParent);

				Occluder occluder = occluderObject[i].occluder;

				//boxの中心座標
				Vector3 pos = new Vector3(UnityEngine.Random.Range(0f, 10f), UnityEngine.Random.Range(0f, 10f), UnityEngine.Random.Range(0f, 10f)) * 2;
				//boxのサイズ
				float sclx = UnityEngine.Random.Range(6f, 12f);
				float scly = UnityEngine.Random.Range(6f, 12f);


				//boxの回転
				float rotx = UnityEngine.Random.Range(0f, 180f);
				float roty = UnityEngine.Random.Range(0f, 180f);
				float rotz = UnityEngine.Random.Range(0f, 180f);
				Quaternion rot = Quaternion.identity;
				occluder.point[0] = rot * new Vector3(0.5f * sclx, 0.5f * scly, 0f) + pos;
				occluder.point[1] = rot * new Vector3(-0.5f * sclx, 0.5f * scly, 0f) + pos;
				occluder.point[2] = rot * new Vector3(-0.5f * sclx, -0.5f * scly, 0f) + pos;
				occluder.point[3] = rot * new Vector3(0.5f * sclx, -0.5f * scly, 0f) + pos;


				occluderObject[i].quad.transform.localPosition = pos;
				occluderObject[i].quad.transform.localRotation = rot;
				occluderObject[i].quad.transform.localScale = new Vector3(sclx, scly, 0.01f);
				occluderObject[i].quad.GetComponent<Renderer>().material.color = occluderColor[i];
			}

			//カリング対象のオブジェクトを設定
			Transform targetParent = new GameObject("target").transform;
			for (int i = 0; i < ObjectMax; i++) {
				cullingTarget[i] = new CullingObject(targetParent);
				ShapeCollision.OBB obb = cullingTarget[i].obb;

				//boxの中心座標
				Vector3 pos = new Vector3 (i % 10, (i % 100) / 10, (i % 1000) / 100);
				obb.center = pos * 2f;

				//boxのサイズ
				float sclx = UnityEngine.Random.Range(0.3f, 0.7f);
				float scly = UnityEngine.Random.Range(0.3f, 0.7f);
				float sclz = UnityEngine.Random.Range(0.3f, 0.7f);
				obb.radius = new float[]{ sclx, scly, sclz };

				//boxの回転
				float rotx = UnityEngine.Random.Range(0f, 180f);
				float roty = UnityEngine.Random.Range(0f, 180f);
				float rotz = UnityEngine.Random.Range(0f, 180f);
				Quaternion rot = Quaternion.Euler(rotx, roty, rotz);
				obb.axis[0] = rot * new Vector3(1f, 0f, 0f);
				obb.axis[1] = rot * new Vector3(0f, 1f, 0f);
				obb.axis[2] = rot * new Vector3(0f, 0f, 1f);

				//boxの外接球の半径
				cullingTarget[i].radius = Mathf.Sqrt(obb.radius[0] * obb.radius[0] + obb.radius[1] * obb.radius[1] + obb.radius[2] * obb.radius[2]);
				cullingTarget[i].isCulling = false;

				//表示用の設定
				cullingTarget[i].cube.transform.localPosition = obb.center;
				cullingTarget[i].cube.transform.localScale = new Vector3(obb.radius[0] * 2f, obb.radius[1] * 2f, obb.radius[2] * 2f);
				cullingTarget[i].cube.transform.localRotation = rot;
			}

			Camera cam = mainCamera_.Camera;
			culling.SetFrustum (cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
		}


		/// <summary>
		/// 実行処理
		/// </summary>
		void Update(){
			if (Input.GetKeyDown(KeyCode.A)) {
				isUpdateMatrix = !isUpdateMatrix;
			}

			//カメラのビュー行列を取得
			if (isUpdateMatrix) {
				matrix = mainCamera_.Camera.worldToCameraMatrix;
			}

			//カリングのための視錐台を計算
			culling.Execute(matrix);

			List<int> occIndex = new List<int>();
			for (int i = 0; i < OccluderMax; i++) {
				bool result = culling.CullingTest(occluderObject[i].occluder);


				//色付けのために遮蔽板がカリング対象にならなかった場合の順番を退避
				if (!result) {
					occIndex.Add(i);
				}
			}
			for (int i = 0; i < ObjectMax; i++) {
				ShapeCollision.OBB obb = cullingTarget[i].obb;

				//カリングテスト
				int index;
				cullingTarget[i].isCulling = culling.CullingTest(obb.center, obb.axis, obb.radius, cullingTarget[i].radius, out index);

				//視錐台の外にあるため表示不要の場合は赤色にする
				if (cullingTarget[i].isCulling){
					if(index < 0){
						cullingTarget[i].cube.GetComponent<Renderer>().material.color = Color.red;
					} else {
						cullingTarget[i].cube.GetComponent<Renderer>().material.color = occluderColor[occIndex[index]];
					}
				} else{
					cullingTarget[i].cube.GetComponent<Renderer>().material.color = Color.cyan;
				}
			}


			assemblyManager_.Execute();
			assemblyManager_.Evaluate();
		}
		/// <summary>
		/// 実行後処理
		/// </summary>
		void LateUpdate() {
			assemblyManager_.LateExecute();

		}

	}

#endif
}
