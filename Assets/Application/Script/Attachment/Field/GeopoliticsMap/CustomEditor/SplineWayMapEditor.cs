#if UNITY_EDITOR
using UnityEngine;

namespace Project.Game.Geopolitics {

	[UnityEditor.CustomEditor(typeof(Map))]
	public class GeopoliticsMapEditor : UnityEditor.Editor {
		public const string TempHierarchy = "Temp";
		Map owner_;

		string path = "Assets/Application/Addressable/Waymap/";
		string filename = "map001.txt";
		private void Awake() {
			owner_ = (Map)base.target;
		}
		/// <summary>
		/// Inspectorを拡張
		/// </summary>
		public override void OnInspectorGUI() {
			//編集用GameObjectの置き場所を探す
			Transform root = owner_.transform.Find(TempHierarchy);

			//場所が無ければ編集前
			if (root == null) {
				base.OnInspectorGUI();
				//自動生成する
				if (GUILayout.Button("Generate")) {
					owner_.RandomBuild();
				}
				//生成結果を保存する
				GUILayout.Label("");
				path = GUILayout.TextField(path);
				filename = GUILayout.TextField(filename);
				if (GUILayout.Button("Save")) {
					string fullpath = Application.dataPath.Replace("Assets", path) + filename;
					owner_.Save(fullpath);
				}
				if (GUILayout.Button("Load")) {
					string fullpath = Application.dataPath.Replace("Assets", path) + filename;
					owner_.Load(fullpath);
					owner_.Build();
				}
			}
		}
	}
}
#endif
