namespace Project.Server {

	[System.Serializable]
	public class UserPlatoonLeaderTable {
		public void SetPrimary(int platoonId, int abreast) {
			this.platoonId = platoonId;
			this.abreast = abreast;
		}
		//ユニーク
		public int platoonId;                               //所属小隊ID(0で無所属)
		public int abreast;                                 //所属分隊ID

		//その他
		public int leaderId;                                //リーダーID
	}
}
