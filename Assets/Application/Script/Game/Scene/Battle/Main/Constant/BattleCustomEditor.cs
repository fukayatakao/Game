#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


[UnityEditor.CustomEditor(typeof(BattleConstConfig))]
public class BattleCustomEditor : UnityEditor.Editor {
	BattleConstConfig owner_;
	private void Awake() {
		owner_ = (BattleConstConfig)base.target;
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

#endif
