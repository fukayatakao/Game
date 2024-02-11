using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// キャラの選択
		/// </summary>
		public static class SelectCharacter {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity actor;
				public CharacterEntity target;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity a, CharacterEntity t) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { actor = a, target = t }),
					(int)MessageGroup.UserEvent
				);
			}
#if DEVELOP_BUILD
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharaMenu menu, MessageObject msg) {
				Data data = (Data)msg.Data;
				menu.Init(data.actor, data.target);
				menu.Show();
				data.actor.SelectCharacter();
			}
#endif
		}
	}
}
