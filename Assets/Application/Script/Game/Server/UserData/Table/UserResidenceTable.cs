using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserResidenceTable {
		public int id;
		public int baseId;  //MstFactoryDataのId
		public Vector3 position;
		public float radius;
	}

}
