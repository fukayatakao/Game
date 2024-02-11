using Project.Lib;
using Project.Master;
using Project.Network;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// アイテム購入
		/// </summary>
		public static class PurchaseItem {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public FacilityType type;
				public int id;
				public int number;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(FacilityType t, int i, int n) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { type = t, id = i, number = n}),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				int price = MasterUtil.GetFacilityPrice(data.type, data.id);
				//所持金が足りない
				if (town.CenterTownhall.HaveParam.Gold < data.number * price)
					return;

				town.CenterTownhall.HaveParam.Gold -= data.number * price;
				TownItemData.I.AddItemNumber((int)data.type, data.id, data.number);
				PurchaseItemCmd.CreateAsync(new PurchaseItemRequest(new ItemData(){id = data.id, type = (int)data.type, number = data.number}));

			}

		}
	}
}
