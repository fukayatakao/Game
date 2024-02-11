using Project.Lib;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// 所持金をセットする
		/// </summary>
		public static class SetGold {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int gold;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int g) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { gold = g }),
					(int)MessageGroup.DebugEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				main.CenterTownhall.HaveParam.Gold = data.gold;
			}
		}

	}
}
