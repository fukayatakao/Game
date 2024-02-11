using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserStorageTable {
		public int id;
		public int baseId;          //MstStorageDataのId
		public Vector3 position;
		public float radius;							//流通半径
	}
}
