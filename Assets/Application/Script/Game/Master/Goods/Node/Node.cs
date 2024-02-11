#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Project.Lib.TreeEditor;
using Project.Lib;

namespace Project.Game.Goods {
	public class Node : Lib.TreeEditor.Node {
		const int INVALID_GOODS = -1;
		public class Control {
			public enum State {
				None,                   //変化なし
				ChangeAmount,			//数量の変化があった
				ChangeGoods,            //構成が変わる変更があった
				DelNode,				//ノードが削除された
				AddNode,				//ノードが追加された
			}
			public Node EditNode { get; private set; }
			public int PrevGoodsId { get; private set; }
			private State state_;
			public void ResetState() {
				state_ = State.None;
			}
			public bool IsChangeAmount() {
				return state_ == State.ChangeAmount;
			}
			public bool IsChangeGoods() {
				return state_ == State.ChangeGoods;
			}
			public bool IsDelete() {
				return state_ == State.DelNode;
			}
			public bool IsAdd() {
				return state_ == State.AddNode;
			}

			public void Add() {
				//未設定goodsとしてノードを追加
				EditNode.child.Add(new Node(INVALID_GOODS, 0));
			}

			public void SetEdit(State state, Node node, int prevGoods) {
				state_ = state;
				EditNode = node;
				PrevGoodsId = prevGoods;
			}

			/// <summary>
			/// 子ノードに同一のidがセットされてたら失敗
			/// </summary>
			public bool CheckSameChild() {
				//ハッシュを使って同じidが2回出てきたら失敗させる
				HashSet<int> hash = new HashSet<int>();
				for (int i = 0, max = EditNode.parent.child.Count; i < max; i++) {
					int id = ((Node)EditNode.parent.child[i]).goodsId;
					//既に登録済のidだったら変更不可
					if (hash.Contains(id)) {
						return true;
					}
					//１回出て来た記録をつける
					hash.Add(id);
				}
				return false;
			}

			public void Rollback() {
				EditNode.goodsPopup = new GoodsGUI(PrevGoodsId);
			}
		}
		public static Control control = new Control();

		GoodsGUI goodsPopup;

		//生産グッズ（id)
		//idから品目
		//idから価格
		//parentがある場合は量も
		public int goodsId { get { return goodsPopup.goodsId; } }
		public int amount { get; private set; }
		public Node(int goodsId, int amount) : base() {
			goodsPopup = new GoodsGUI(goodsId);
			this.amount = amount;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override bool Draw(Vector2 pos) {
			int prevGoods = goodsPopup.goodsId;
			bool result = false;
			//オフセット+中央揃え
			GUI.BeginGroup(new Rect(innerRect_.position + pos, innerRect_.size));
			Color col = GUI.color;
			GUI.color = new Color(196f / 255f, 196f / 255f, 255f / 255f);

			//ノードの内側の矩形（ノード内のローカル座標系）
			Rect Inner = NodeConst.InnerRect;
			//現在選択中でない場合は表示のみ
			if (!current_) {
				DrawBox(Inner, new Color(96f / 255f, 196f / 255f, 255f / 255f));
				goodsPopup.Label(NodeConst.LabelRect);
			//選択中ノードはグッズの変更、数量の変更が可能
			} else {
				GUIUtil.BeginChangeCheck();
				Rect popupRect = new Rect(new Vector2(Inner.x, Inner.y), new Vector2(Inner.size.x, Inner.size.y - NodeConst.LabelTextSize - 2));
				goodsPopup.Popup(popupRect);
				if (GUIUtil.EndChangeCheck()) {
					control.SetEdit(Control.State.ChangeGoods, this, prevGoods);
				}


			}

			if(level_ == 1) {
				GUI.color = new Color(255f / 255f, 196f / 255f, 255f / 255f);
				Rect r = new Rect(new Vector2(Inner.x, Inner.y + Inner.size.y - NodeConst.LabelTextSize - 2), new Vector2(Inner.size.x, NodeConst.LabelTextSize));
				GUIUtil.BeginChangeCheck();
				amount = EditorGUI.IntField(r, "", amount, NodeConst.LabelStyle);
				if (GUIUtil.EndChangeCheck()) {
					control.SetEdit(Control.State.ChangeAmount, this, prevGoods);
				}
			}else if (current_) {
				GUI.color = new Color(196f / 255f, 196f / 255f, 196f / 255f);
				Rect r = new Rect(new Vector2(Inner.x, Inner.y + Inner.size.y - NodeConst.LabelTextSize - 2), new Vector2(Inner.size.x, NodeConst.LabelTextSize));
				GUIUtil.BeginChangeCheck();
				EditorGUI.LabelField(r, "100", NodeConst.LabelStyle);
				if (GUIUtil.EndChangeCheck()) {
					control.SetEdit(Control.State.ChangeAmount, this, prevGoods);
				}
			}
			GUI.color = col;
			GUI.EndGroup();


			if (current_) {
				GUI.BeginGroup(new Rect(outerRect_.position + pos - new Vector2(0f, NodeConst.ButtonHeight), outerRect_.size));
				Rect plusRect = new Rect(new Vector2(outerRect_.size.x - (NodeConst.ButtonWidth * 2), NodeConst.DeployRect.position.y), NodeConst.DeployRect.size);
				if (GUI.Button(plusRect, "＋")) {
					control.SetEdit(Control.State.AddNode, this, prevGoods);
				}
				Rect minusRect = new Rect(new Vector2(outerRect_.size.x - (NodeConst.ButtonWidth), NodeConst.DeployRect.position.y), NodeConst.DeployRect.size);
				if (GUI.Button(minusRect, "－")) {
					control.SetEdit(Control.State.DelNode, this, prevGoods);
				}

				GUI.EndGroup();
			}




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
