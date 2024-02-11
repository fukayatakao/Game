using System.Collections.Generic;
using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserMarketTable {
		public int id;
		public int baseId;  //MstMarketDataのId
		public Vector3 position;
		public float radius;							//流通半径

		//DBにしたときの保存方法は考える必要がある。。。
		public List<int> goodsIds;
	}

}
