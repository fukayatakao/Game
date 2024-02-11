using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserStorageTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserStorageTable> SelectAll() {
			return UserDB.Instance.storageTable;
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static UserStorageTable SelectById(int id) {
			return UserDB.Instance.storageTable.Find(x => x.id == id);
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static void Insert(UserStorageTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.storageTable.Exists(x => x.id == table.id), "storage table id is dupplicate:" + table.id);
			UserDB.Instance.storageTable.Add(table);
		}
		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id) {
			UserStorageTable table = SelectById(id);
			UserDB.Instance.storageTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserStorageTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id);
			Insert(table);
		}


	}
}
