#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Game {

	[UnityEditor.CustomEditor(typeof(TownConstConfig))]
	public class TownCustomEditor : UnityEditor.Editor {
		TownConstConfig owner_;
		private void Awake() {
			owner_ = (TownConstConfig)base.target;
		}

		/// <summary>
		/// Inspectorを拡張
		/// </summary>
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			//自動生成する
			if (GUILayout.Button("Reset")) {
				owner_.Load();
				EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
			}
		}
	}
}
#endif
