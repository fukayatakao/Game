using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class ChainData
	{
		//---- レスポンス変数定義 ----
		public int senderId; //送る側Id
		public int recieverId; //受け側Id
		public float distance; //距離
		public bool valid; //有効

	}

}
