using UnityEngine;

namespace Project.Server {
	[System.Serializable]
	public class UserTownhallTable {
		public int id;
		public int baseId;          //MstStorageDataのId
		public Vector3 position;
		public int gold;            //ゲーム内通貨
		public int tax;				//消費税率
		public int logistics;		//流通
	}
}
