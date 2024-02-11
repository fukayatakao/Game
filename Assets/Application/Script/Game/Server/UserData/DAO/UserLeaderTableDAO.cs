using System.Collections.Generic;

namespace Project.Server {
	public static class UserLeaderTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserLeaderTable> SelectAll() {
			return UserDB.Instance.leaderTable;
		}

		/// <summary>
		/// 小隊所属単位のキャラ取得
		/// </summary>
		public static UserLeaderTable SelectById(int id) {
			return UserDB.Instance.leaderTable.Find(x => x.id == id);
		}


	}
}
