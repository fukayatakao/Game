namespace Project.Server {
	[System.Serializable]
	public class UserPlatoonTable {
		public void SetPrimary(int id) {
			this.id = id;
		}
		public int id;                                      //ユニークID
		public string name;                                 //小隊名
		public int experience;								//部隊熟練度
		public int unitId1;                                 //ユニット種別ID
		public int unitId2;                                 //ユニット種別ID
		public int unitId3;                                 //ユニット種別ID
	}

}
