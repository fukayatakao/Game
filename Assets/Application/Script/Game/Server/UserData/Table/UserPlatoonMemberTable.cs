namespace Project.Server {
	[System.Serializable]
	public class UserPlatoonMemberTable {
		public void SetPrimary(int platoonId, int abreast, int number) {
			this.platoonId = platoonId;
			this.abreast = abreast;
			this.number = number;
		}
		//ユニーク
		public int platoonId;                               //所属小隊ID(0で無所属)
		public int abreast;                                 //所属分隊ID
		public int number;                                  //列内の順番

		//その他
		public int charaId;                                 //キャラID
	}



}
