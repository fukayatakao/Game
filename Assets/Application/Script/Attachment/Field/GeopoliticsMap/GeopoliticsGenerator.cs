using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Geopolitics {
	public static class Generator {
		/// <summary>
		/// 次のノード候補リストを作る
		/// </summary>
		static List<Node> CreateNextList(Dictionary<int, List<Node>> nodeList, Node node, int layer) {
			//ゴールと一つ手前のノードは必ず繋がる
			if (layer == nodeList.Count - 1)
				return nodeList[layer];
			List<Node> list = new List<Node>();
			for (int i = 0; i < nodeList[layer].Count; i++) {
				if ((node.position - nodeList[layer][i].position).sqrMagnitude < 20 * 20) {
					list.Add(nodeList[layer][i]);
				}
			}

			return list;
		}

		public static Data Generate(int layer, int cell, Rect area) {
			Dictionary<int, List<Node>> layerNodesDict = new Dictionary<int, List<Node>>();
			//Nodeを作る/////////////////////////
			//分割した領域ごとに１つのノードを作る
			float layerSize = (area.yMax - area.yMin) / layer;
			float cellSize = (area.xMax - area.xMin) / cell;

			int counter = 0;
			//Startノード
			{
				layerNodesDict[0] = new List<Node>();
				float z = area.yMin + layerSize * 0.5f;
				for (int j = 0; j < cell; j++) {
					float xMin = area.xMin + cellSize * j;
					float xMax = area.xMin + cellSize * (j + 1);

					float x = UnityEngine.Random.Range(xMin, xMax);
					Node n = new Node() { id = counter, position = new Vector3(x, 0, z) };
					counter++;
					layerNodesDict[0].Add(n);
				}
			}
			//Goalノード
			{
				int last = layer - 1;
				layerNodesDict[last] = new List<Node>();
				float z = area.yMin + layerSize * (last + 0.5f);
				int j = cell / 2;

				float x = area.xMin + cellSize * j;
				Node n = new Node() { id = counter, position = new Vector3(x, 0, z) };
				counter++;
				layerNodesDict[last].Add(n);
			}

			for (int i = 1; i < layer - 1; i++) {
				layerNodesDict[i] = new List<Node>();
				float zMin = area.yMin + layerSize * i;
				float zMax = area.yMin + layerSize * (i + 1);
				for (int j = 0; j < cell; j++) {
					float xMin = area.xMin + cellSize * j;
					float xMax = area.xMin + cellSize * (j + 1);

					float x = UnityEngine.Random.Range(xMin, xMax);
					float z = UnityEngine.Random.Range(zMin, zMax);
					Node n = new Node() { id = counter, position = new Vector3(x, 0, z) };
					counter++;
					layerNodesDict[i].Add(n);
				}
			}

			//Nodeを繋げる/////////////////////////
			List<KeyValuePair<Node, Node>> route = new List<KeyValuePair<Node, Node>>();
			for (int i = 0; i < layerNodesDict.Count - 1; i++) {
				for (int j = 0; j < layerNodesDict[i].Count; j++) {
					//次のレイヤ以降のノードと１～２繋がる様にする
					List<Node> list = CreateNextList(layerNodesDict, layerNodesDict[i][j], i + 1);
					if (list.Count == 0)
						continue;
					//startとgoalが同じ経路が作られないようにgoalに使ったnodeを保存して抽選されたら弾く
					HashSet<Node> goalHash = new HashSet<Node>();
					int add = UnityEngine.Random.Range(0, 100) > 70 ? 0 : 1;
					for (int c = 0, max = 1 + add; c < max; c++) {
						Node g = list[UnityEngine.Random.Range(0, list.Count)];
						//同じgoalが抽選されたら無効にする
						if (goalHash.Contains(g)) {
							continue;
						}
						goalHash.Add(g);
						layerNodesDict[i][j].next.Add(g.id);
						route.Add(new KeyValuePair<Node, Node>(layerNodesDict[i][j], g));
					}
				}
			}

			//一旦繋げた後にstart->goalの経路として冗長なものを枝狩りする
			//一つ前のlayerとの接続がないノードと経路を削除する
			for (int i = 1; i < layerNodesDict.Count - 1; i++) {
				for (int j = 0, max = layerNodesDict[i].Count; j < max; j++) {
					//調査ノードをゴールとする経路が存在しない=一つ前のレイヤーとの接続がない
					Node node = layerNodesDict[i][j];
					if (!route.Exists(x => x.Value == node)) {
						route.RemoveAll(x => x.Key == node);
						layerNodesDict[i].RemoveAt(j);
						j--;
						max--;
					}
				}
			}


			List<Way> ways = new List<Way>();
			List<Node> starts = new List<Node>();
			Node goal;
			List<Node> nodes = new List<Node>();

			//必要なノードと経路だけになったのでオブジェクトを配置していく
			for (int i = 0, max = layerNodesDict[0].Count; i < max; i++) {
				starts.Add(layerNodesDict[0][i]);
			}
			{
				goal = layerNodesDict[layerNodesDict.Count - 1][0];
			}
			for (int i = 1, max = layerNodesDict.Count - 1; i < max; i++) {
				for (int j = 0, max2 = layerNodesDict[i].Count; j < max2; j++) {
					nodes.Add(layerNodesDict[i][j]);
				}
			}

			//経路を繋げる
			for (int i = 0, max = route.Count; i < max; i++) {
				ways.Add(new Way(route[i].Key, route[i].Value));
			}

			//必要な経路だけになったら傾きを計算する
			for (int i = 0, max = ways.Count; i < max; i++) {
				Way current = ways[i];
				//調査対象のstartをgoalとする経路があるか調べる
				Way prev = ways.Find(x => x.goal.position == current.start.position);
				//現経路の前の経路がない場合はstratとgoalのベクトルで補完
				if (prev == null) {
					current.start.vector = current.goal.position - current.start.position;
					//経路がある場合は前の経路の開始位置とgoalのベクトルで補完
				} else {
					current.start.vector = current.goal.position - prev.start.position;
				}

				//@note startとgoal両方に傾きを入れると歪み過ぎるのでgoalの傾き計算はなし
				current.goal.vector = current.goal.position - current.start.position;
				current.CalculateLength();
			}



#if UNITY_EDITOR
			//実行中はdirtyフラグ建ては無視
			if (!Application.isPlaying) {
				//dirtyフラグを建てないとCtrl+Sで保存できない
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
			}
#endif

			Data data = new Data();
			data.ways = ways;
			data.starts = starts;
			data.goal = goal;
			data.nodes = nodes;

			return data;
		}
	}
}
