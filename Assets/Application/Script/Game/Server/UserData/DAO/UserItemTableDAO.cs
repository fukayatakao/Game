using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Project.Server {
	public static class UserItemTableDAO {
		/// <summary>
		/// データ取得
		/// </summary>
		public static List<UserItemTable> SelectAll() {
			return UserDB.Instance.itemTable;
		}
		/// <summary>
		/// データ取得
		/// </summary>
		public static UserItemTable SelectById(int id, int type) {
			return UserDB.Instance.itemTable.Find(x => x.id == id && x.type == type);
		}
		/// <summary>
		/// データ取得
		/// </summary>
		public static List<UserItemTable> SelectByType(int type) {
			return new List<UserItemTable>(UserDB.Instance.itemTable.Where(x => x.type == type));
		}
		/// <summary>
		/// データを追加
		/// </summary>
		public static void Insert(UserItemTable table) {
			//ユニークIDが重複してたらAssert
			Debug.Assert(!UserDB.Instance.itemTable.Exists(x => x.id == table.id && x.type == table.type), string.Format("item table id is duplicate:id={0} type={1}", table.id, table.type));
			UserDB.Instance.itemTable.Add(table);
			//仮状態ではjsonで保存してるので順番がずれないようにSortかける
			UserDB.Instance.itemTable.Sort((a, b) => a.id - b.id);
		}

		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(int id, int type) {
			UserItemTable table = SelectById(id, type);
			UserDB.Instance.itemTable.Remove(table);
		}

		/// <summary>
		/// データを更新
		/// </summary>
		public static void Update(UserItemTable table) {
			//なんちゃってなので削除->新規作成でヨシ
			Delete(table.id, table.type);
			Insert(table);
		}
	}
}
