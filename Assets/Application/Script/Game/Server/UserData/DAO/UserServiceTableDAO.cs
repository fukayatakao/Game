using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserServiceTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserServiceTable> SelectAll() {
			return UserDB.Instance.serviceTable;
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static UserServiceTable SelectById(int id) {
			return UserDB.Instance.serviceTable.Find(x => x.id == id);
		}

		/// <summary>
		/// データを追加
		/// </summary>
		public static void Insert(UserServiceTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.serviceTable.Exists(x => x.id == table.id), "factory table id is dupplicate:" + table.id);
			UserDB.Instance.serviceTable.Add(table);
			//仮状態ではjsonで保存してるので順番がずれないようにSortかける
			UserDB.Instance.serviceTable.Sort((a, b) => a.id - b.id);
		}

		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id) {
			UserServiceTable table = SelectById(id);
			UserDB.Instance.serviceTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserServiceTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id);
			Insert(table);
		}

	}
}
