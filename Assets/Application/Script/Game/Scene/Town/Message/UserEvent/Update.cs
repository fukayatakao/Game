using Project.Lib;
using Project.Network;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// ゲーム更新
		/// </summary>
		public static class Update {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public TownData townData;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(TownData data) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ townData = data }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				town.LoadTown(data.townData);
			}
		}
	}
}
