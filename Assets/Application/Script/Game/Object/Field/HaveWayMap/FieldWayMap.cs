using Project.Game.Geopolitics;
using Project.Lib;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// フィールドの経路マップ
	/// </summary>
	public class FieldWayMap : MonoPretender {
		const string OBJECT_NAME = "WayMap";
		UnityEngine.Object waymapData_;
		UnityEngine.Object startPrefab_;
		UnityEngine.Object goalPrefab_;
		UnityEngine.Object nodePrefab_;

		Geopolitics.Map waymap_;

		//nodeIdとNode本体のDictionary
		public Dictionary<int, Node> nodeDict = new Dictionary<int, Node>();
		//開始ノード+終端ノードで経路を探すための2重Dictionary
		public Dictionary<int, Dictionary<int, Way>> wayDict = new Dictionary<int, Dictionary<int, Way>>();
		//攻撃側の初期配置可能ノード
		public List<Node> starts { get { return waymap_.data.starts; } }
		//防衛側のボスがいるノード
		public Node goal { get { return waymap_.data.goal; } }
		//それ以外の中間ノード
		public List<Node> nodes { get { return waymap_.data.nodes; } }


		public void RandomBuild() {
			waymap_.RandomBuild();
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			AddressableAssist.UnLoadAsset(waymapData_);
			AddressableAssist.UnLoadAsset(startPrefab_);
			AddressableAssist.UnLoadAsset(goalPrefab_);
			AddressableAssist.UnLoadAsset(nodePrefab_);
		}

		public async Task LoadAsync(GameObject root, string asset) {
			waymapData_ = await AddressableAssist.LoadAssetAsync(asset);
			startPrefab_ = await AddressableAssist.LoadAssetAsync(AddressableDefine.Address.START_PREFAB);
			goalPrefab_ = await AddressableAssist.LoadAssetAsync(AddressableDefine.Address.GOAL_PREFAB);
			nodePrefab_ = await AddressableAssist.LoadAssetAsync(AddressableDefine.Address.NODE_PREFAB);


			waymap_ = root.AddComponent<Geopolitics.Map>();
			waymap_.data = Geopolitics.Data.FromBase64String((waymapData_ as TextAsset).text);
			waymap_.startPrefab = startPrefab_ as GameObject;
			waymap_.goalPrefab = goalPrefab_ as GameObject;
			waymap_.nodePrefab = nodePrefab_ as GameObject;
			waymap_.Build();

			//開始ノードと終端ノードから経路を取得するためのdictionaryを作る
			for (int i = 0, max = waymap_.data.ways.Count; i < max; i++) {
				Way way = waymap_.data.ways[i];
				if (!wayDict.ContainsKey(way.startNodeId))
					wayDict[way.startNodeId] = new Dictionary<int, Way>();
				(wayDict[way.startNodeId])[way.goalNodeId] = way;
			}

			//@todo 予めprefabに入れておくようにする
			//ノードをクリックできるようにコリジョンを置く
			for (int i = 0, max = starts.Count; i < max; i++) {
				GameObject obj = new GameObject("node collision");
				UnityUtil.SetLayer(obj.transform, (int)UnityLayer.Layer.Collision);
				var coll = obj.AddComponent<CapsuleCollider>();
				coll.height = 5f;
				obj.transform.localPosition = StrategyUtil.Grounding(starts[i].position);
				obj.AddComponent<NodePortal>().Id = starts[i].id;
			}



			//@todo 審議
			for (int i = 0, max = waymap_.data.starts.Count; i < max; i++) {
				nodeDict[waymap_.data.starts[i].id] = waymap_.data.starts[i];
			}
			nodeDict[waymap_.data.goal.id] = waymap_.data.goal;

			for (int i = 0, max = waymap_.data.nodes.Count; i < max; i++) {
				nodeDict[waymap_.data.nodes[i].id] = waymap_.data.nodes[i];
			}

		}

		/// <summary>
		/// シーンルートに置かれている経路情報を取得する
		/// </summary>
		public void Init(GameObject[] root) {
			for (int i = 0, max = root.Length; i < max; i++) {
				//コンポーネントを探す
				if (root[i].name != OBJECT_NAME)
					continue;
				waymap_ = root[i].GetComponent<Geopolitics.Map>();
				if (waymap_ == null)
					continue;

				break;
			}
			//経路情報のコンポーネントがなかったらエラー
			Debug.Assert(waymap_ != null, "not found waymap component");

		}
		/*public void Setup() { 
			//開始ノードと終端ノードから経路を取得するためのdictionaryを作る
			for(int i = 0, max = waymap_.data.ways.Count; i < max; i++) {
				Way way = waymap_.data.ways[i];
				if (!wayDict.ContainsKey(way.startNodeId))
					wayDict[way.startNodeId] = new Dictionary<int, Way>();
				(wayDict[way.startNodeId])[way.goalNodeId] = way;
			}

			//ノードをクリックできるようにコリジョンを置く
			for(int i = 0, max = starts.Count; i < max; i++) {
				GameObject obj = new GameObject("node collision");
				UnityUtil.SetLayer(obj.transform, (int)UnityLayer.Layer.Collision);
				var coll = obj.AddComponent<CapsuleCollider>();
				coll.height = 5f;
				obj.transform.localPosition = StrategyUtil.Grounding(starts[i].position);
				obj.AddComponent<NodePortal>().Id = starts[i].id;
			}



			//@todo 審議
			for(int i = 0, max = waymap_.data.starts.Count; i < max; i++) {
				nodeDict[waymap_.data.starts[i].id] = waymap_.data.starts[i];
			}
			nodeDict[waymap_.data.goal.id] = waymap_.data.goal;

			for (int i = 0, max = waymap_.data.nodes.Count; i < max; i++) {
				nodeDict[waymap_.data.nodes[i].id] = waymap_.data.nodes[i];
			}

		}*/

		public Dictionary<int, Way> StartWay(int id) {
			return wayDict[id];
		}
		
	}
}
