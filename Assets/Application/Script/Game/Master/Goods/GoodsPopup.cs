#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Project.Lib.TreeEditor;
using Project.Mst;

namespace Project.Game.Goods {
	public class GoodsGUI {
		private static List<int> ids_;
		private static string[] goodsName_;
		private static Dictionary<int, MstGoodsData> goodsDict_;
		public static void Init(Dictionary<int, MstGoodsData> goodsDict) {
			goodsDict_ = goodsDict;
			goodsName_ = goodsDict_.Values.Select(x => x.DbgName).ToArray();
			ids_ = new List<int>(goodsDict_.Values.Select(x => x.Id));
		}

		public int goodsId { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GoodsGUI(int id) {
			goodsId = id;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Popup(Rect rect) {
			int index = ids_.FindIndex(x => x == goodsId);
			index = EditorGUI.Popup(rect, index, goodsName_, NodeConst.NodeStyle);
			goodsId = ids_[index];

		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Label(Rect rect) {
			EditorGUI.LabelField(rect, goodsDict_[this.goodsId].DbgName, NodeConst.LabelStyle);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Popup() {
			int index = ids_.FindIndex(x => x == goodsId);
			index = EditorGUILayout.Popup(index, goodsName_, NodeConst.NodeStyle);
			goodsId = ids_[index];

		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Label() {
			EditorGUILayout.LabelField(goodsDict_[this.goodsId].DbgName, NodeConst.LabelStyle);
		}

	}
}
#endif
