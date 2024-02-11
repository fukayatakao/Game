using Project.Mst;
using System.Collections.Generic;

namespace Project.Server {
	public class ProductData {
		public int Goods { get; private set; }
		public int Amount;
		public int Grade;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductData(int goods) {
			Goods = goods;
		}

		/// <summary>
		/// 同一グッズでGradeが違うものが増える場合の計算
		/// </summary>
		public void Push(int amount, int grade) {
			int total = Amount + amount;
			//0除算を避ける
			if (total > 0) {
				//(現在量xグレード) + (増加する量xグレード) / (現在量+増加する量) = 新規平均グレード
				Grade = (Grade * Amount + grade * amount) / total;
			} else {
				Grade = 0;
			}
			Amount = total;
		}
	}

	public static class GoodsTool {
		//Goodsの処理順
		static Dictionary<int, int> priority_ = new Dictionary<int, int>();
		public static Dictionary<int, int> PriorityMap { get { return priority_; } }

		//生産Goods, 原料Goods, 消費Value
		static Dictionary<int, Dictionary<int, int>> goodsResource_ = new Dictionary<int, Dictionary<int, int>>();
		public static Dictionary<int, Dictionary<int, int>> GoodsResource { get { return goodsResource_; } }

		//Goods, 品目
		static Dictionary<int, int> goodsArticle_ = new Dictionary<int, int>();
		public static Dictionary<int, int> GoodsArticle { get { return goodsArticle_; } }

		//Goods, 価格
		static Dictionary<int, int> goodsPrice_ = new Dictionary<int, int>();
		public static Dictionary<int, int> GoodsPrice { get { return goodsPrice_; } }


		class Node {
			public int id;
			public List<Node> next = new List<Node>();		//自分を原料とするGoodsのNode
		}

		public static void Init() {
			InitPriority();
			InitGoodsResource();
			goodsArticle_ = new Dictionary<int, int>();
			var mst = BaseDataManager.GetList<MstGoodsData>();
			foreach(var goods in mst) {
				goodsArticle_[goods.Id] = goods.Article;
				goodsPrice_[goods.Id] = goods.Price;
			}
		}
		/// <summary>
		/// 生産時の処理順を計算
		/// </summary>
		private static void InitPriority() {
			var mst = BaseDataManager.GetList<MstGoodsData>();
			List<Node> roots = new List<Node>();
			Dictionary<int, Node> nodes = new Dictionary<int, Node>();
			//必要リソースのないGoodsを起点にする。priorityは0。
			for (int i = 0, max = mst.Count; i < max; i++) {
				MstGoodsData data = mst[i];
				Node n = new Node() { id = data.Id };
				nodes[data.Id] = n;
				if (data.ResourceId1 == 0 && data.ResourceId2 == 0 && data.ResourceId3 == 0) {
					roots.Add(n);
				}

				priority_[data.Id] = -1;
			}
			//必要リソースでNodeを繋げる
			for (int i = 0, max = mst.Count; i < max; i++) {
				MstGoodsData data = mst[i];
				if (data.ResourceId1 != 0) {
					nodes[data.ResourceId1].next.Add(nodes[data.Id]);
				}
				if (data.ResourceId2 != 0) {
					nodes[data.ResourceId2].next.Add(nodes[data.Id]);
				}
				if (data.ResourceId3 != 0) {
					nodes[data.ResourceId3].next.Add(nodes[data.Id]);
				}
			}

			//それぞれの起点からゴールのない幅優先探索して最長距離をpriorityとする
			for (int i = 0, max = roots.Count; i < max; i++) {
				SetPriority(roots[i], 0);
			}

		}


		/// <summary>
		/// 再帰的に降りて行ってpriorityをセット
		/// </summary>
		private static void SetPriority(Node n, int priority) {
			//遠い方で上書き
			if(priority_[n.id] < priority)
				priority_[n.id] = priority;
			priority++;
			for(int i = 0, max = n.next.Count; i < max; i++) {
				SetPriority(n.next[i], priority);
			}
		}

		/// <summary>
		/// 生産時の処理順を計算
		/// </summary>
		private static void InitGoodsResource() {
			var dict = BaseDataManager.GetDictionary<int, MstGoodsData>();
			foreach(var mst in dict.Values) {
				goodsResource_[mst.Id] = new Dictionary<int, int>();
				if(mst.ResourceId1 != 0) {
					goodsResource_[mst.Id][mst.ResourceId1] = mst.UseAmount1;
				}
				if (mst.ResourceId2 != 0) {
					goodsResource_[mst.Id][mst.ResourceId2] = mst.UseAmount2;
				}
				if (mst.ResourceId3 != 0) {
					goodsResource_[mst.Id][mst.ResourceId3] = mst.UseAmount3;
				}
			}

		}
	}
}
