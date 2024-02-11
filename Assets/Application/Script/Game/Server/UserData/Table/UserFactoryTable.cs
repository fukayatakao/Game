using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserFactoryTable {
		public int id;
		public int baseId;								//MstFactoryDataのId
		public Vector3 position;						//配置座標
		public float radius;							//流通半径

		public int product;                             //生産するGoods
		public int grade;                               //生産物のグレード
		public int output;                              //生産量
		public int worker;                              //最大労働者数
	}
}
