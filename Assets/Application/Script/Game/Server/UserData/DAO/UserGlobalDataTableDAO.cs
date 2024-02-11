namespace Project.Server {
	public static class UserGlobalDataTableDAO {
		/// <summary>
		/// データ取得
		/// </summary>
		public static UserGlobalDataTable Select() {
			return UserDB.Instance.globalDataTable;
		}
		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserGlobalDataTable table) {
			UserDB.Instance.globalDataTable = table;
		}
	}
}
