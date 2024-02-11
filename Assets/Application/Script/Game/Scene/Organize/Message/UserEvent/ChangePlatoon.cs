using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// 小隊の変更
		/// </summary>
		public static class ChangePlatoon {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int index;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int i) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { index = i }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.ChangePlatoon(data.index);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharaMenu menu, MessageObject msg) {
				menu.Hide();
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterSelectControl control, MessageObject msg) {
				control.UnSelectLastTarget();
			}
		}
	}
}
