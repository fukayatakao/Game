using System.Collections.Generic;

namespace Project.Server {
	[System.Serializable]
	public class UserStrategyMapTable {
		public void SetPrimary(int userId) {
			this.userId = userId;
		}
		public int userId;                                      //ユニークID
		public int mapId;                                       //記録したマップID
		public int turn;                                        //現在ターン
		public List<int> available;


		public string json;										//経路の座標とか曲がり具合とかはサーバでは不要なはずなので全部jsonで入れてしまおう
	}
}
