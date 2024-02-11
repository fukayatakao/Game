using System.Collections.Generic;

namespace Project.Server {
	public static class UserCharacterTableDAO {
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

		/// <summary>
		/// その施設で労働しているキャラ取得
		/// </summary>
		public static List<UserMemberTable> SelectByWorkId(int id) {
			return UserDB.Instance.memberTable.FindAll(x => x.workId == id);
		}

		/// <summary>
		/// その住居に住んでいるキャラ取得
		/// </summary>
		public static List<UserMemberTable> SelectByStayId(int id) {
			return UserDB.Instance.memberTable.FindAll(x => x.id == id);
		}

		/// <summary>
		/// キャラ情報の更新
		/// </summary>
		public static void Update(List<UserMemberTable> tables) {
			for (int i = 0, max = tables.Count; i < max; i++) {
				Update(tables[i]);
			}
		}

		/// <summary>
		/// キャラ情報の更新
		/// </summary>
		public static void Update(UserMemberTable table) {
			for(int i = 0, max = UserDB.Instance.memberTable.Count; i < max; i++) {
				if(UserDB.Instance.memberTable[i].id == table.id) {
					UserDB.Instance.memberTable[i] = table;
				}
			}
		}

	}
}
