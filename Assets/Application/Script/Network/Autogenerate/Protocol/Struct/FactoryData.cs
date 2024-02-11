using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class FactoryData
	{
		//---- レスポンス変数定義 ----
		public int id; //識別子
		public int baseId; //FactoryBaseのid
		public Vector3 position; //座標
		public float radius; //流通半径
		public int product; //生産グッズ
		public int grade; //生産物のグレード
		public int output; //生産量
		public int worker; //最大労働者数

	}

}
