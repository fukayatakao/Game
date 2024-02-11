using Project.Master;
using Project.Network;
using UnityEngine;

#if USE_OFFLINE

namespace Project.Server
{
	public partial class DummyServer
	{
		/// <summary>
		/// アイテム購入通信
		/// </summary>
		public static Response PurchaseItemOffline(Request req) {
			var request = req as PurchaseItemRequest;
			var item = request.item;
			PurchaseItemResponse response = new PurchaseItemResponse();

			UserItemTable table = UserItemTableDAO.SelectById(item.id, item.type);
			//新規追加
			if (table == null) {
				UserItemTableDAO.Insert(new UserItemTable(){id = item.id, type = item.type, number = item.number});
			} else {
				table.number += item.number;
				UserItemTableDAO.Update(table);
			}

			UserTownhallTable townhallTable = UserTownhallTableDAO.Select();
			int price = MasterUtil.GetFacilityPrice((FacilityType)item.type, item.id);
			townhallTable.gold -= (price * item.number);
			Debug.Assert(townhallTable.gold >= 0, "gold settlement error:" + townhallTable.gold);

			UserDB.Save();
			return response;
		}
	}
}
#endif
