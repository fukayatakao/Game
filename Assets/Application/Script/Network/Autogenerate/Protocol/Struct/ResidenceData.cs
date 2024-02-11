using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class ResidenceData
	{
		//---- レスポンス変数定義 ----
		public int id; //識別子
		public int baseId; //ResidenceBaseのid
		public Vector3 position; //座標
		public float radius; //流通半径
		public float satify; //満足度

	}

}
