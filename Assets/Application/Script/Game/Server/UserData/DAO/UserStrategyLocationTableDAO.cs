using System.Collections.Generic;

namespace Project.Server {
	public static class UserStrategyLocationTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserStrategyLocationTable> SelectAll() {
			return UserDB.Instance.strategyLocationTable;
		}
		/// <summary>
		/// 個別データ取得
		/// </summary>
		public static List<UserStrategyLocationTable> SelectByUserIdMapId(int userId, int mapId) {
			return UserDB.Instance.strategyLocationTable.FindAll(x => x.userId == userId && x.mapId == mapId);
		}
	}
}
