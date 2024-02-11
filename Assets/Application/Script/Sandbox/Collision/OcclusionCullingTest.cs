
/*
namespace Project.Lib {
#if UNITY_EDITOR && DEVELOP_BUILD
	/// <summary>
	/// コリジョンのデバッグ表示
	/// </summary>
	public class OcclusionCullingTest : MonoBehaviour {
		[SerializeField]
		Camera cullingCamera_;

		const int OccluderMax = 10;
		Occluder[] occluder = new Occluder[OccluderMax];

		const int ObjectMax = 1000;
		ConstrainBox[] box = new ConstrainBox[ObjectMax];

		OcclusionCulling culling = new OcclusionCulling();



		void Awake(){
			for (int i = 0; i < OccluderMax; i++) {
				occluder[i] = new Occluder();
			}
			for (int i = 0; i < ObjectMax; i++) {
				box[i] = new ConstrainBox ();
				Vector3 pos = new Vector3 (i % 10, (i % 100) / 10, (i % 1000) / 100);
				box[i].SetCenter(pos * 2f);
				box[i].SetRadius(0.5f, 0.5f, 0.5f);

				box [i].Constrain (transform);
			}


			culling.SetFrustum (cullingCamera_.fieldOfView, cullingCamera_.aspect, cullingCamera_.nearClipPlane, cullingCamera_.farClipPlane
			);
		}


		/// <summary>
		/// コリジョン表示
		/// </summary>
		void Update(){
			if (cullingCamera_ == null)
				return;
			culling.LateExecute(cullingCamera_.worldToCameraMatrix);
			for (int i = 0; i < OccluderMax; i++) {
				bool result = culling.CullingTest (occluder [i]);
			}
			for (int i = 0; i < ObjectMax; i++) {
				box[i].LateExecute();
				bool result = false;

				culling.CullingTest (box[i]);

				if (result)
					box[i].color = Color.red;
				else
					box[i].color = Color.cyan;
			}



		}

	}

#endif
}
*/
