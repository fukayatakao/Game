using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class MarketData
	{
		//---- レスポンス変数定義 ----
		public int id; //識別子
		public int baseId; //MarketBaseのid
		public Vector3 position; //座標
		public float radius; //流通半径
		public List<int> goodsIds; //取り扱いグッズ

	}

}
