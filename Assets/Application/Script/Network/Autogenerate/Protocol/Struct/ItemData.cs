using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class ItemData
	{
		//---- レスポンス変数定義 ----
		public int classify; //アイテム分類
		public int type; //アイテム種別
		public int id; //アイテムId
		public int number; //アイテム数

	}

}
