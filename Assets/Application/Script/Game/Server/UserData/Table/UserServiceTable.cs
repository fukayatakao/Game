using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserServiceTable {
		public int id;
		public int baseId;          //MstStorageDataのId
		public Vector3 position;
		public float radius;							//流通半径
		public int product;                             //生産するGoods
		public int grade;                               //生産物のグレード
		public int output;                              //生産量
		public int worker;                              //最大労働者数
	}
}
