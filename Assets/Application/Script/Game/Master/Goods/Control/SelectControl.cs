#if UNITY_EDITOR
using UnityEngine;
using Project.Lib;


namespace Project.Game.Goods {
	/// <summary>
	/// ノード選択
	/// </summary>
	public class SelectControl : IHaveControl {
		//操作中か
		bool enable_;

		// 操作プライオリティ
		public int Priority { get { return (int)Project.Lib.TreeEditor.OperationPriority.SelectNode; } }

		GoodsTreeEditor window_;
		Lib.TreeEditor.Node select_;
		/// <summary>
		/// ウィンドウをセットアップ
		/// </summary>
		public SelectControl(GoodsTreeEditor window, UserOperation operation) {
			window_ = window;
			//自分を制御振り分け機能に登録要求
			operation.Register(this, false);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (Event.current.type != EventType.MouseDown)
				return false;


			Lib.TreeEditor.Node root = window_.root;
			select_ = SelectNode(root, Event.current.mousePosition - window_.basePoint);

			if (select_ == null)
				return false;
			window_.current = select_;
			window_.Repaint();
			return true;
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {

			enable_ = true;
		}
		/// <summary>
		/// 制御終了
		/// </summary>
		public bool IsEnd() {
			return !enable_;
		}
		/// <summary>
		/// 制御却下
		/// </summary>
		public void Reject() {
			enable_ = false;
			window_.Repaint();
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			if (!enable_)
				return;
			switch (Event.current.type) {
			case EventType.MouseUp:
				Reject();
				break;
			default:
				break;
			}
		}

		Lib.TreeEditor.Node SelectNode(Lib.TreeEditor.Node node, Vector2 pos) {
			if (node.innerRect_.Contains(pos)) {
				return node;
			}
			if (node.Collapse)
				return null;
			for(int i = 0, max = node.child.Count; i < max; i++) {
				Lib.TreeEditor.Node n = SelectNode(node.child[i], pos);
				if (n != null)
					return n;
			}
			return null;
		}

	}
}
#endif
