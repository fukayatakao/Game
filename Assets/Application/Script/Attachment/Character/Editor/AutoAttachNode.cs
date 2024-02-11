using UnityEngine;
using UnityEditor;

namespace Project.Lib {
    /// <summary>
    /// キャラクターの行動評価関数
    /// </summary>
    public static class CharacterNodeEdit {
		static string path = "Assets/Application/Addressable/Character/Model";

		const string Head = "Head";
		const string Body = "Spine";
		const string LHand = "Hand_L";
		const string RHand = "Hand_R";
		const string LFoot = "Foot_L";
		const string RFoot = "Foot_R";


		/// <summary>
		/// キャラクターのノードを登録する
		/// </summary>
		[MenuItem("Editor/CharacterNode/AttachAll")]
        public static void AttachAll() {
			//modelプレハブのリストを取得
			string[] models = System.IO.Directory.GetFiles(path.Replace("Assets", Application.dataPath), "*.prefab");

			for(int i = 0, max = models.Length; i< max; i++){
				AttachImpl(models[i].Replace(Application.dataPath, "Assets"));
			}

			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// コンポーネントにTransformを登録
		/// </summary>
		private static void AttachImpl(string assetPath) {
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

			//キャラクターのノードアタッチを取得。なければ追加。
			CharacterNode characterNode = obj.GetComponent<CharacterNode>();
			if (characterNode == null) {
				characterNode = obj.AddComponent<CharacterNode>();
			}

			Transform[] node = obj.GetComponentsInChildren<Transform>(obj);
			for (int i = 0; i < node.Length; i++) {
				if (node[i].name.EndsWith(Head)) {
					characterNode.Head = node[i];
				}
				if (node[i].name.EndsWith(Body)) {
					characterNode.Body = node[i];
				}
				if (node[i].name.EndsWith(LHand)) {
					characterNode.LHand = node[i];
				}
				if (node[i].name.EndsWith(RHand)) {
					characterNode.RHand = node[i];
				}
				if (node[i].name.EndsWith(LFoot)) {
					characterNode.LFoot = node[i];
				}
				if (node[i].name.EndsWith(RFoot)) {
					characterNode.RFoot = node[i];
				}
			}
			EditorUtility.SetDirty(obj);

		}
	}
}
