using Project.Lib;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// 生産処理
		/// </summary>
		public static class ShowSupply {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Factory factory;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Factory f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { factory = f }),
					(int)MessageGroup.DebugEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				//town.LinkMap.ShowSupplyLink(data.factory, town.Factories);
			}
		}

	}
}
