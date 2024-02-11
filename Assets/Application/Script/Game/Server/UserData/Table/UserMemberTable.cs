namespace Project.Server {
	[System.Serializable]
	public class UserMemberTable {
		public void SetPrimary(int memberId) {
			this.id = memberId;
		}
		//ユニーク
		public int id;                                 //ID

		//その他
		public string name;                                 //名前
		public bool generate;								//キャラ画像が生成されたオリジナルのものか
		public string portrait;                             //キャラ画像
		public int species;                                 //種族ID
		public int ability1;                                //アビリティ種別ID1=>パラメータばらつかせる用
		public int ability2;                                //アビリティ種別ID1
		public int ability3;                                //アビリティ種別ID1

		public int phase;                                   //五行の元素

		public int stayId;                                  //住居Id
		public int staySlotId;                              //住居スロット番号
		public int workId;                                  //職場Id
		public int workSlotId;                              //職場スロット番号
		public int grade;									//Townでの生活水準
		public int[] gradeHistory = new int[GameConst.Town.HISTORY_MAX];				//単位時間n回分のグレード履歴。現在のグレードはこの値の平均

	}

}
