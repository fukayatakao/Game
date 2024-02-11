using Project.Lib;

namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// 住人への供給リンク表示
		/// </summary>
		public static class ShowSupplyResidence {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Residence residence;
				public int goodsId;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Residence r, int id) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ residence = r, goodsId  = id}),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;
				//選択を解除してからモード切替。次に選択したときは切り替わったモードで矢印表示がされる
				//town.UserOperation.Reset();

				//town.LinkMap.ShowSupplyResidence(data.residence, data.goodsId);

			}

		}
	}
}
