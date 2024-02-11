using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserResidenceTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserResidenceTable> SelectAll() {
			return UserDB.Instance.residenceTable;
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static UserResidenceTable SelectById(int id) {
			return UserDB.Instance.residenceTable.Find(x => x.id == id);
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static void Insert(UserResidenceTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.residenceTable.Exists(x => x.id == table.id), "residence table id is dupplicate:" + table.id);
			UserDB.Instance.residenceTable.Add(table);
		}
		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id) {
			UserResidenceTable table = SelectById(id);
			UserDB.Instance.residenceTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserResidenceTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id);
			Insert(table);
		}

	}
}
