#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


namespace Project.Lib.TreeEditor {
	public class TreeEditor : EditorWindow {
		/// <summary>
		/// 編集ダイアログのオープン
		/// </summary>
		//[MenuItem("Editor/TreeEditor", false, 98)]
		private static void Open() {
			TreeEditor window = EditorWindow.GetWindow<TreeEditor>(false, "TreeEditor");
		}
		/// <summary>
		/// ウィンドウがアクティブになったとき
		/// </summary>
		private void OnEnable() {
			Init();
		}
		private void OnDestroy() {
		}
		/// <summary>
		/// 初期化
		/// </summary>
		protected virtual void Init() {
			InterNodeRect = new Dictionary<int, List<Rect>>();
			operation_ = new UserOperation();
			SetupOperation();
			operation_.SetDefault((int)OperationPriority.MainView);
			Create();
		}

		protected virtual void SetupOperation() {
			operationExecute_.Add(new MainControl(this, operation_).Execute);
			operationExecute_.Add(new NodeSelectControl(this, operation_).Execute);
		}

		/// <summary>
		/// 新規作成
		/// </summary>
		protected void Create() {
			root = new Node();
			current = root;
			ResetLayout();
		}
		/// <summary>
		/// 表示をリセット
		/// </summary>
		protected void ResetLayout() {
			scale = 1f;
			root.position = new Vector2(0, 0);
			basePoint = new Vector2(-root.position.x + position.size.x / scale / 2, root.position.y);
		}

		public Vector2 basePoint;
		public float scale_;
		public float scale{
			get{ return scale_; }
			set{
				scale_ = value;
				NodeConst.Init(scale_);
				Layout();
			}
		}
		public Node current;

		protected List<System.Action> operationExecute_ = new List<Action>();
		protected UserOperation operation_;

		public Node root;
		//ノード間の間の矩形、ノードを別のノードの間に移動させるときなどに使用
		public Dictionary<int, List<Rect>> InterNodeRect;
		/// <summary>
		/// 表示処理
		/// </summary>
		protected virtual void OnGUI() {
			if (KeyControl())
				return;
			//GUI.TextFieldで日本語入力を可能にす
			Input.imeCompositionMode = IMECompositionMode.On;

			NodeConst.InitStyle(scale);
			//ノードの描画範囲を指定
			GUI.BeginGroup(new Rect(0, 0, position.size.x, position.size.y));

			if (current != null) {
				bool layout = current.DrawFrame(basePoint);
				//折り畳みが発生したらツリーを再計算
				if (layout) {
					Layout();
					return;
				}
			}

			if (DrawNode(root)) {
				Layout();
				return;
			}
			//ロードダイアログのように前に他のウィンドウが出てる時も反応してしまうので対策
			for (int i = 0, max = operationExecute_.Count; i < max; i++) {
				operationExecute_[i]();
			}
			operation_.Execute();
			GUI.EndGroup();
		}
		/// <summary>
		/// Node描画
		/// </summary>
		private bool DrawNode(Node node) {
			bool result = false;
			if(node.parent != null) {
				Vector3 start = new Vector3(node.position.x + basePoint.x, node.position.y + basePoint.y, 0f);
				Vector3 end = new Vector3(node.parent.position.x + basePoint.x, node.parent.position.y + basePoint.y + NodeConst.Size.y, 0f);
				Vector3[] P = new Vector3[]
				{
					start,end
				};
				Handles.DrawPolyLine(P);
			}
			bool layout = node.Draw(basePoint);
			if (layout) {
				return true;
			}
			if (node.Collapse)
				return false;
			for(int i = 0, max = node.child.Count; i < max; i++) {
				result |= DrawNode(node.child[i]);
			}

			return result;
		}

		/// <summary>
		/// ノードを追加する
		/// </summary>
		protected void AddNode(Node parent, Node child) {
			if (!parent.IsChild())
				return;
			child.parent = parent;
			parent.child.Add(child);
			Layout();
		}
		/// <summary>
		/// ノードを削除する
		/// </summary>
		protected void DelNode(Node node) {
			node.parent.child.Remove(node);
			Layout();
		}
		/// <summary>
		/// 親ノードを移動
		/// </summary>
		public void MoveParents(Node node, int parent) {
			//親が同一の場合は移動なし
			if (node.parent.Id == parent) {
				return;
			}
			Node parentNode = SerchNode(root, parent);
			//親ノードが子を持てない場合は無視
			if (!parentNode.IsChild())
				return;
			//循環するような移動は不可
			if (DetectParentNode(parentNode, node.Id))
				return;
			//親子逆転させるような移動は無視
			Node n = SerchNode(node, parentNode.Id);
			if (n != null)
				return;

			node.parent.child.Remove(node);

			parentNode.child.Add(node);
			node.parent = parentNode;
			Layout();
		}
		/// <summary>
		/// ノードの隙間に移動
		/// </summary>
		public void MoveParts(Node node, int parent, int index) {
			//親が同一の場合は移動なし
			if (node.Id == parent) {
				return;
			}
			Node parentNode = SerchNode(root, parent);
			//循環するような移動は不可
			if (DetectParentNode(parentNode, node.Id))
				return;

			//親が移動しない場合は移動前に自分が居なくなることによる要素数減少の対策を入れる
			if (node.parent.Id == parent) {
				if(node.parent.child.FindIndex(n => n == node) < index) {
					index--;
				}
			}
			node.parent.child.Remove(node);


			Debug.Assert(parentNode != null, "not found parent node:" + parent);
			if(index >= 0 && index < parentNode.child.Count) {
				parentNode.child.Insert(index, node);
			} else {
				if(index < 0) {
					parentNode.child.Insert(0, node);
				} else {
					parentNode.child.Add(node);
				}
			}
			node.parent = parentNode;

			Layout();
		}

		/// <summary>
		/// ツリーを巡ってIDが一致するノードを探す
		/// </summary>
		private Node SerchNode(Node node, int id) {
			if (node.Id == id)
				return node;

			for(int i = 0, max = node.child.Count; i < max; i++) {
				Node n = SerchNode(node.child[i], id);
				if (n != null)
					return n;
			}
			return null;
		}

		/// <summary>
		/// 親を辿ってidが見つかるか
		/// </summary>
		private bool DetectParentNode(Node node, int id) {
			Node n = node;
			while (n != null) {
				if (n.Id == id)
					return true;
				n = n.parent;
			}
			return false;
		}


		Dictionary<Node, int> nodeWeight = new Dictionary<Node, int>();
		private void Layout() {
			nodeWeight.Clear();
			Weighting(root);

			InterNodeRect.Clear();
			root.SetRect();
			Layout(root, root.position, 1);
			CalcInter(root);
		}

		/// <summary>
		/// ノードごとの重みを計算する
		/// </summary>
		private int Weighting(Node node) {
			if (node.Collapse) {
				nodeWeight[node] = 1;
				return 1;
			}
			int weight = node.child.Count + 1;
			for (int i = 0, max = node.child.Count; i < max; i++) {
				weight += Weighting(node.child[i]) - 1;
			}

			Debug.Assert(!nodeWeight.ContainsKey(node), "already node exists");
			nodeWeight[node] = weight;

			return weight;
		}

		/// <summary>
		/// 重みをもとにノード配置を計算
		/// </summary>
		private void Layout(Node node, Vector2 pos, int depth) {
			//nodeの全体の重み=必要な幅
			int total = nodeWeight[node] - 1;

			//階層ごとにノードの高さx2の幅を開ける
			float y = pos.y + NodeConst.Size.y * 2 * depth;
			int weight = 0;
			float left = node.position.x - (total * NodeConst.Size.x / 2);

			if (node.Collapse)
				return;

			for (int i = 0, max = node.child.Count; i < max; i++) {
				int w = nodeWeight[node.child[i]];
				float x = left + NodeConst.Size.x * weight + NodeConst.Size.x * w *0.5f;

				node.child[i].position = new Vector2(x, y);
				Layout(node.child[i], pos, depth + 1);

				weight += w;
			}
		}

		/// <summary>
		/// 狭間領域の計算
		/// </summary>
		private void CalcInter(Node node) {
			if (node.Collapse) {
				InterNodeRect[node.Id] = new List<Rect>();
				return;
			}

			List<Rect> list = new List<Rect>();
			//先頭分を生成
			if(node.child.Count > 0) {
				float left = node.child[0].innerRect_.x - node.child[0].innerRect_.width * 0.25f;
				float right = left + node.child[0].innerRect_.width * 0.5f;
				float top = node.child[0].innerRect_.y;
				float height = node.child[0].innerRect_.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}
			//ノード間の矩形を生成
			for (int i = 0, max = node.child.Count - 1; i < max; i++) {
				float left = node.child[i].innerRect_.x + node.child[i].innerRect_.width * 0.75f;
				float right = node.child[i + 1].innerRect_.x + node.child[i + 1].innerRect_.width * 0.25f;
				float top = node.child[i].innerRect_.y;
				float height = node.child[i].innerRect_.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}
			//後端分を生成
			if (node.child.Count > 0) {
				int index = node.child.Count - 1;
				float left = node.child[index].innerRect_.x + node.child[index].innerRect_.width * 0.75f;
				float right = left + node.child[index].innerRect_.width * 0.5f;
				float top = node.child[index].innerRect_.y;
				float height = node.child[index].innerRect_.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}

			InterNodeRect[node.Id] = list;
			for (int i = 0, max = node.child.Count; i < max; i++) {
				CalcInter(node.child[i]);
			}

		}



		public Node clipboard_ = null;
		private bool ctrl_ = false;

		private bool KeyControl() {
			Event e = Event.current;
			//ctrlキーチェック
			if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl) {
				ctrl_ = true;
			}
			if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl) {
				ctrl_ = false;
			}

			//ctrl+c
			if (ctrl_ && e.type == EventType.KeyDown && e.keyCode == KeyCode.C) {
				clipboard_ = current;
			}
			//ctrl+v
			if (ctrl_ && e.type == EventType.KeyDown && e.keyCode == KeyCode.V) {
				CopyNode(clipboard_, current);
			}
			Repaint();
			return false;
		}

		/// <summary>
		/// Nodeをコピーする
		/// </summary>
		private void CopyNode(Node src, Node dest) {
			//srcと同じ内容をdestの子にぶら下げる
			AddNode(dest, new Node(src));
			ResetLayout();
		}

	}
}
#endif
