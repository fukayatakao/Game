#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib.TreeEditor {
	public static class NodeConst {
		public static void Init(float scale) {
			LabelTextSize = (int)(14 * scale);
			BorderSize = (int)(10 * scale);
			ButtonWidth = (int)(24 * scale);
			ButtonHeight = (int)(24 * scale);
			Size = new Vector2(120f * scale, 100f * scale);
			OuterRect = new Rect(new Vector2(0, 0), new Vector2(Size.x + BorderSize, Size.y + BorderSize));
			InnerRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2), new Vector2(Size.x, Size.y));
			LabelRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2 + (Size.y - LabelTextSize) * 0.5f), new Vector2(Size.x, LabelTextSize));
			DeployRect = new Rect(0 + 0 * ButtonWidth, 0f, ButtonWidth, ButtonHeight);
		}
		/// <summary>
		/// StyleはOnGUIの中で設定しないとエラーになるので分離
		/// </summary>
		public static void InitStyle(float scale) {
			if(scale_ != scale) {
				NodeStyle = new GUIStyle {
					normal =
					{
						background = (Texture2D)Resources.Load("NodeFrame"),
						textColor = Color.black,
					},
					border = new RectOffset(NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize),

					alignment = TextAnchor.MiddleCenter,
					wordWrap = true,
					clipping = TextClipping.Clip,
					fontSize = NodeConst.LabelTextSize,
				};

				LabelStyle = new GUIStyle
				{
					normal =
					{
						background = (Texture2D)Resources.Load("NodeFrame"),
						textColor = Color.black,
					},
					border = new RectOffset(NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize),
					alignment = TextAnchor.MiddleCenter,
					wordWrap = true,
					clipping = TextClipping.Clip,
					fontSize = NodeConst.LabelTextSize,
				};

			}
		}

		public static int LabelTextSize;
		public static int BorderSize;
		public static Vector2 Size;
		public static Rect OuterRect;
		public static Rect InnerRect;
		public static Rect LabelRect;

		public static int ButtonWidth;
		public static int ButtonHeight;

		public static Rect DeployRect;

		public static float scale_ = -1;
		public static GUIStyle NodeStyle;
		public static GUIStyle LabelStyle;
	}


	public class Node {
		public int Id;
		public string Label;
		public Node parent;
		public List<Node> child = new List<Node>();
		public bool IsChild() { return true; }
		//子ノードの折り畳みフラグ
		public bool Collapse;
		private Color color_ =  new Color(196f / 255f, 196f / 255f, 255f / 255f);
		private static UniqueCounter Counter;

		protected bool current_;
		protected int level_;
		//表示座標
		Vector2 position_;
		public Vector2 position {
			get{
				return position_;
			}
			set {
				position_ = value;
				SetRect();
			}
		}

		public Rect innerRect_{ get; private set; }
		public Rect outerRect_{ get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Node() {
			if (Counter == null) {
				Counter = new UniqueCounter();
			}
			Id = Counter.GetUniqueId();
			Label = Id.ToString();
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Node(Node org) {
			Id = Counter.GetUniqueId();
			Label = org.Label;

			for(int i = 0, max = org.child.Count; i < max; i++) {
				Node node = new Node(org.child[i]);
				child.Add(node);
				node.parent = this;
			}
		}

		public void SetRect() {
			outerRect_ = new Rect(position.x - (NodeConst.Size.x + NodeConst.BorderSize) / 2, position.y, NodeConst.Size.x + NodeConst.BorderSize, NodeConst.Size.y + NodeConst.BorderSize);
			innerRect_ = new Rect(position.x - (NodeConst.Size.x + NodeConst.BorderSize) / 2, position.y, NodeConst.Size.x + NodeConst.BorderSize, NodeConst.Size.y + NodeConst.BorderSize);
		}

		/// <summary>
		/// box表示
		/// </summary>
		protected void DrawBox(Rect rect, Color color) {
			Color col = GUI.color;
			GUI.color = color;
			GUI.Box(rect, "", NodeConst.NodeStyle);
			Label = GUI.TextField(NodeConst.LabelRect, Label, NodeConst.LabelStyle);
			GUI.color = col;
		}

		/// <summary>
		/// 選択中の外枠表示
		/// </summary>
		public bool DrawFrame(Vector2 pos) {
			bool result = false;
			//選択状態の外枠表示
			GUI.BeginGroup(new Rect(outerRect_.position + pos, outerRect_.size));
			DrawBox(NodeConst.OuterRect, new Color(96f / 255f, 196f / 255f, 255f / 255f));
			GUI.EndGroup();

			//折り畳み操作ボタン
			if (!Collapse && child.Count > 0) {
				GUI.BeginGroup(new Rect(outerRect_.position + pos - new Vector2(0f, NodeConst.ButtonHeight), outerRect_.size));
				if (GUI.Button(NodeConst.DeployRect, "∧")) {
					Collapse = !Collapse;
					result = true;
				}
				GUI.EndGroup();
			}
			//選択中の外枠を描いたということはこのノードが選択中のはず
			current_ = true;
			for(int i = 0, max = child.Count; i < max; i++) {
				child[i].level_ = 1;
			}

			return result;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public virtual bool Draw(Vector2 pos) {
			bool result = false;
			//オフセット+中央揃え
			GUI.BeginGroup(new Rect(innerRect_.position + pos, innerRect_.size));
			DrawBox(NodeConst.InnerRect, color_);
			GUI.EndGroup();
			//折り畳みの展開ボタン
			if (Collapse && child.Count > 0) {
				Vector2 p = innerRect_.position + pos + new Vector2(0f, innerRect_.size.y);
				if(GUI.Button(new Rect(p, new Vector2(innerRect_.size.x, NodeConst.LabelTextSize)), "・・・")) {
					Collapse = false;
					result = true;
				}
			}

			current_ = false;
			level_ = 0;
			return result;
		}
	}
}
#endif
