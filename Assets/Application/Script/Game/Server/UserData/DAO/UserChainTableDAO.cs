using UnityEngine;
using System.Collections.Generic;

namespace Project.Server {
	public static class UserChainTableDAO {
		/// <summary>
		/// 全データ取得
		/// </summary>
		public static List<UserChainTable> SelectAll() {
			return UserDB.Instance.chainTable;
		}
		/// <summary>
		/// 全データ取得
		/// </summary>
		/// <remarks>
		/// 距離でソート
		/// </remarks>
		public static List<UserChainTable> SelectAllOrderByDistancesq() {
			List<UserChainTable> list = new List<UserChainTable>(UserDB.Instance.chainTable);
			list.Sort((a, b) => a.distance.CompareTo(b.distance));
			return list;
		}

		/// <summary>
		/// 送り側と受け側のidから検索
		/// </summary>
		public static List<UserChainTable> SelectBySenderReciever(int senderId, int recieverId) {
			return UserDB.Instance.chainTable.FindAll(x =>
					x.senderId == senderId &&
					x.recieverId == recieverId
					);
		}
		/// <summary>
		/// 送り側か受け側のidから検索
		/// </summary>
		public static List<UserChainTable> SelectBySenderOrReciever(int facilityId) {
			return UserDB.Instance.chainTable.FindAll(x =>
				x.senderId == facilityId ||
				x.recieverId == facilityId
			);
		}

		/// <summary>
		/// データを作成
		/// </summary>
		public static void Insert(List<UserChainTable> tables) {
#if DEVELOP_BUILD
			//ユニークIDが重複してたらAssert
			for(int i = 0, max = tables.Count; i < max; i++) {
				Debug.Assert(!UserDB.Instance.chainTable.Exists(
					x => x.senderId == tables[i].senderId && x.recieverId == tables[i].recieverId),
					"chain table is dupplicate:" + tables[i]);
			}
#endif
			UserDB.Instance.chainTable.AddRange(tables);
		}
		/// <summary>
		/// データを削除
		/// </summary>
		public static void Delete(List<(int, int)> keys) {
			for(int i = 0, max = keys.Count; i < max; i++) {
				int senderId = keys[i].Item1;
				int recieverId = keys[i].Item2;
				List<UserChainTable> tables = SelectBySenderReciever(senderId, recieverId);
				foreach(UserChainTable table in tables) {
					UserDB.Instance.chainTable.Remove(table);
				}
			}
		}
		/// <summary>
		/// データを削除
		/// </summary>
		public static void DeleteBySenderOrReciever(int id) {
			List<UserChainTable> tables = SelectBySenderOrReciever(id);
			foreach(UserChainTable table in tables) {
				UserDB.Instance.chainTable.Remove(table);
			}
		}

	}
}
