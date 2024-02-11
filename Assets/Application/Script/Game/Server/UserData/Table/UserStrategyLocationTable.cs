namespace Project.Server {
	[System.Serializable]
	public class UserStrategyLocationTable {
		public void SetPrimary(int userid, int mapId, int nodeId) {
			this.userId = userid;
			this.mapId = mapId;
			this.nodeId = nodeId;
		}
		public int userId;                                      //UserStrategyMapTableのID
		public int mapId;                                       //マップのID
		public int nodeId;										//ノードのID
		public int platoonId;                                   //ノードにいる部隊ID
		public int attitude;									//守備側:1 攻撃側:2
	}

}
