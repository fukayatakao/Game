using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	public class MarketParam : MonoPretender {
		public int[] NegativeGoodsIds;

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup(int capacity, List<int> goodsIds=null) {
			NegativeGoodsIds = new int[capacity];
			if (goodsIds == null)
				return;
			NegativeGoodsIds = goodsIds.ToArray();
		}


	}
}
