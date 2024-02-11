using System.Collections.Generic;
using System.Linq;

namespace Project.Server {
	public static class UserPlatoonUnitTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserPlatoonMemberTable> SelectAll() {
			return UserDB.Instance.platoonMemberTable;
		}

		/// <summary>
		/// 小隊所属単位のキャラ取得
		/// </summary>
		public static List<UserPlatoonMemberTable> SelectByPlatoonId(int id) {
			return new List<UserPlatoonMemberTable>(UserDB.Instance.platoonMemberTable.Where(x => x.platoonId == id));
		}
		/// <summary>
		/// 小隊所属単位のキャラ取得
		/// </summary>
		public static List<UserPlatoonMemberTable> SelectByCharacterId(int platoonId, int charaId) {
			return new List<UserPlatoonMemberTable>(UserDB.Instance.platoonMemberTable.Where(x => x.platoonId == platoonId).Where(x => x.charaId == charaId));
		}



		/// <summary>
		/// 小隊所属のキャラ更新
		/// </summary>
		public static void UpdateCharacters(int platoonId, List<UserPlatoonMemberTable> characters) {
			UserDB.Instance.platoonMemberTable.RemoveAll(x => x.platoonId == platoonId);
			UserDB.Instance.platoonMemberTable.AddRange(characters);
		}


	}
}
