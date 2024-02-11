using Project.Lib;


namespace Project.Game {
    public static partial class BattleMessage {
		/// <summary>
		/// ユーザー制御の受け先変更を要求
		/// </summary>
		public static class SquadRotation {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Power team;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power t) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { team = t }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.Platoon[(int)data.team].Rotation();
			}
		}
		/// <summary>
		/// ユーザー制御の受け先変更を要求
		/// </summary>
		public static class SquadForeSwap {
            //メッセージ種別のID
            private static int ID = -1;
            //送付するデータ内容
            private class Data {
				public Power team;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power t) {
                MessageSystem.Broadcast(
                    new MessageObject(ID, new Data { team = t }),
                    (int)MessageGroup.UserEvent
                );
            }

            /// <summary>
            /// メッセージを受信して処理
            /// </summary>
            private static void Recv(BattleMain main, MessageObject msg) {
                Data data = (Data)msg.Data;

				main.Platoon[(int)data.team].ForeSwap();
			}
		}
		/// <summary>
		/// ユーザー制御の受け先変更を要求
		/// </summary>
		public static class SquadAftSwap {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Power team;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power t) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { team = t }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.Platoon[(int)data.team].AftSwap();
			}
		}
	}
}
