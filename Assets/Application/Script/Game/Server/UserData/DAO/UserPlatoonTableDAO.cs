using System.Collections.Generic;
using System.Linq;

namespace Project.Server {
	public static class UserPlatoonTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserPlatoonTable> SelectAll() {
			return UserDB.Instance.platoonTable;
		}
		/// <summary>
		/// 個別データ取得
		/// </summary>
		public static UserPlatoonTable SelectById(int id) {
			return UserDB.Instance.platoonTable.Find(x => x.id == id);
		}
		/// <summary>
		/// 複数データ取得
		/// </summary>
		public static List<UserPlatoonTable> SelectByIds(List<int> ids) {
			return new List<UserPlatoonTable>(UserDB.Instance.platoonTable.Where(x=> ids.Any(id => id == x.id)));
		}
	}
}
