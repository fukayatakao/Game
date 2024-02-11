using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class TownhallData
	{
		//---- レスポンス変数定義 ----
		public int id; //識別子
		public int baseId; //FactoryBaseのid
		public Vector3 position; //座標
		public int tax; //税率
		public int gold; //所持金
		public int logistics; //流通

	}

}
