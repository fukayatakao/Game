using System.Collections.Generic;

namespace Project.Server {
	public static class UserStrategyMapTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserStrategyMapTable> SelectAll() {
			return UserDB.Instance.strategyMapTable;
		}
		/// <summary>
		/// 個別データ取得
		/// </summary>
		public static UserStrategyMapTable SelectByUserId(int userId) {
			return UserDB.Instance.strategyMapTable.Find(x => x.userId == userId);
		}
		/// <summary>
		/// 個別データ追加
		/// </summary>
		public static void Insert(UserStrategyMapTable table) {
			UserDB.Instance.strategyMapTable.Add(table);
		}
	}
}
