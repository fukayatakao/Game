namespace Project.Server {
	[System.Serializable]
	public class UserLeaderTable {
		//その他
		public int id;                                      //管理ID
		public int leaderMasterId;                          //リーダーマスターID
		public int limitbreak;								//
		public int lv;                                      //レベル
		public int rarity;                                  //レアリティ
		public int skillALv;								//スキルa
		public int skillBLv;                                //スキルb
		public int skillCLv;                                //スキルc
		public int skillDLv;                                //スキルc

		public int grade;                                   //Townでの生活水準
		public int[] gradeHistory = new int[GameConst.Town.HISTORY_MAX];             //単位時間n回分のグレード履歴。現在のグレードはこの値の平均

	}

}
