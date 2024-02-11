using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserMarketTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserMarketTable> SelectAll() {
			return UserDB.Instance.marketTable;
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static UserMarketTable SelectById(int id) {
			return UserDB.Instance.marketTable.Find(x => x.id == id);
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static void Insert(UserMarketTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.marketTable.Exists(x => x.id == table.id), "market table id is duplicate:" + table.id);
			UserDB.Instance.marketTable.Add(table);
		}
		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id) {
			UserMarketTable table = SelectById(id);
			UserDB.Instance.marketTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserMarketTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id);
			Insert(table);
		}


	}
}
