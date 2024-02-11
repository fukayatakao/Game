using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserFactoryTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserFactoryTable> SelectAll() {
			return UserDB.Instance.factoryTable;
		}

		/// <summary>
		/// idから取得
		/// </summary>
		public static UserFactoryTable SelectById(int id) {
			return UserDB.Instance.factoryTable.Find(x => x.id == id);
		}

		/// <summary>
		/// データを追加
		/// </summary>
		public static void Insert(UserFactoryTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.factoryTable.Exists(x => x.id == table.id), "factory table id is duplicate:" + table.id);
			UserDB.Instance.factoryTable.Add(table);
			//仮状態ではjsonで保存してるので順番がずれないようにSortかける
			UserDB.Instance.factoryTable.Sort((a, b) => a.id - b.id);
		}

		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id) {
			UserFactoryTable table = SelectById(id);
			UserDB.Instance.factoryTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserFactoryTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id);
			Insert(table);
		}

	}
}
