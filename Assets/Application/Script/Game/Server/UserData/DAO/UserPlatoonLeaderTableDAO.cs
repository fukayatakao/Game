using System.Collections.Generic;

namespace Project.Server {
	public static class UserPlatoonLeaderTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserPlatoonLeaderTable> SelectAll() {
			return UserDB.Instance.platoonLeaderTable;
		}
		/// <summary>
		/// 小隊所属単位のキャラ取得
		/// </summary>
		public static List<UserPlatoonLeaderTable> SelectByPlatoonId(int id) {
			return UserDB.Instance.platoonLeaderTable.FindAll(x => x.platoonId == id);
		}
		/// <summary>
		/// 小隊所属のリーダー更新
		/// </summary>
		public static void UpdateLeaders(int platoonId, List<UserPlatoonLeaderTable> leaders) {
			UserDB.Instance.platoonLeaderTable.RemoveAll(x => x.platoonId == platoonId);
			UserDB.Instance.platoonLeaderTable.AddRange(leaders);
		}

	}
}
