using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using System.IO;
using System.Text;

namespace Project.Game.Geopolitics {
	public class Map : MonoBehaviour {
		public Data data;

		public GameObject nodePrefab;
		public GameObject startPrefab;
		public GameObject goalPrefab;


		const int LineDiv = 100;
		const string LineRoot = "Lines";
		const string NodeRoot = "Nodes";

		public void RandomBuild() {
			data = Generator.Generate(6, 4, new Rect(-30, -30, 60, 60));

			BuildNodes(data.starts, data.goal, data.nodes);
			BuildLine(data.ways);
		}
		public void Build() {
			BuildNodes(data.starts, data.goal, data.nodes);
			BuildLine(data.ways);
		}
		/// <summary>
		/// 線表示を構築する
		/// </summary>
		public void BuildNodes(List<Node> starts, Node goal, List<Node> nodes) {
			UnityUtil.DestroyGameObject(transform.Find(NodeRoot));
			Transform nodeRoot = UnityUtil.InstantiateChild(transform, NodeRoot).transform;

			//ライン描画を置く階層を作る
			nodeRoot.SetParent(transform);
			nodeRoot.gameObject.layer = (int)UnityLayer.Layer.UI;

			for(int i = 0, max = starts.Count; i < max; i++) {
				CreateNode(starts[i].position, nodeRoot, startPrefab, "start:" + starts[i].id.ToString());
			}
			CreateNode(goal.position, nodeRoot, goalPrefab, "goal:" + goal.id.ToString());
			for (int i = 0, max = nodes.Count; i < max; i++) {
				CreateNode(nodes[i].position, nodeRoot, nodePrefab, "nodes:" + nodes[i].id.ToString());
			}
		}

		private void CreateNode(Vector3 pos, Transform parent, GameObject prefab, string caption) {
			GameObject obj = UnityUtil.InstantiateChild(parent, prefab);
			obj.transform.SetParent(parent);
			obj.transform.localPosition = pos;
			obj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			obj.name = caption;
			obj.SetActive(true);
		}

		/// <summary>
		/// 線表示を構築する
		/// </summary>
		public void BuildLine(List<Way> ways) {
			UnityUtil.DestroyGameObject(transform.Find(LineRoot));
			Transform lines = UnityUtil.InstantiateChild(transform, LineRoot).transform;

			//ライン描画を置く階層を作る
			lines.SetParent(transform);
			lines.gameObject.layer = (int)UnityLayer.Layer.UI;

			//スプラインに合わせて線描画するように設定
			for (int i = 0, max = ways.Count; i < max; i++) {
				Way spline = ways[i];
				GameObject l = new GameObject("line" + i);
				l.transform.SetParent(lines);
				l.layer = (int)UnityLayer.Layer.UI;
				CreateLineRenderer(l, spline);
			}
		}
		/// <summary>
		/// ライン描画の設定
		/// </summary>
		private void CreateLineRenderer(GameObject line, Way spline) {
			LineRenderer renderer = line.AddComponent<LineRenderer>();
			renderer.material = Resources.Load<Material>("Line/Line");
			renderer.startWidth = 0.1f;
			renderer.endWidth = 0.1f;
			renderer.useWorldSpace = false;
			List<Vector3> posList = new List<Vector3>();
			// 線分を作成
			for (var i = 0; i <= LineDiv; i++) {
				posList.Add(spline.Evaluate((float)i / LineDiv));
			}
			renderer.positionCount = posList.Count;
			renderer.SetPositions(posList.ToArray());

		}
#if UNITY_EDITOR
		/// <summary>
		/// 経路情報の保存
		/// </summary>
		public void Save(string path) {
			string text = Data.ToBase64String(data);
			using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8)) {
				sw.WriteLine(text);
			}
		}
		/// <summary>
		/// 経路情報の保存
		/// </summary>
		public void Load(string path) {
			using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
				string text = sr.ReadToEnd();
				data = Data.FromBase64String(text);
			}
		}

#endif

	}
}
