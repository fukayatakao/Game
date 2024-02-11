namespace Project.Server {
	public static class UserTownhallTableDAO {
		/// <summary>
		/// データ取得
		/// </summary>
		public static UserTownhallTable Select() {
			return UserDB.Instance.townhallTable;
		}
		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserTownhallTable table) {
			UserDB.Instance.townhallTable = table;
		}
	}
}
