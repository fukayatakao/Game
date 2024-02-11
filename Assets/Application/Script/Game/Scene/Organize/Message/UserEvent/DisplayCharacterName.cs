using Project.Lib;

namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// キャラの選択
		/// </summary>
		public static class DisplayCharacterName {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public bool flag;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(bool f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data{ flag = f }),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterNameText nameText, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (data.flag) {
					nameText.Show();
				} else {
					nameText.Hide();
				}
			}
		}
	}
}
