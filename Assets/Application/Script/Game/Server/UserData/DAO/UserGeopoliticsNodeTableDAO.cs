using System.Collections.Generic;

namespace Project.Server {
	public static class UserGeopoliticsNodeTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserMemberTable> SelectAll() {
			return UserDB.Instance.memberTable;
		}

		/// <summary>
		/// 小隊所属単位のキャラ取得
		/// </summary>
		public static UserMemberTable SelectById(int id) {
			return UserDB.Instance.memberTable.Find(x => x.id == id);
		}


	}
}
