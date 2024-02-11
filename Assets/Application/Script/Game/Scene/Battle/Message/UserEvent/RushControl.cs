using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// ラッシュをする列制御
		/// </summary>
		public static class RushControl {
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
					new MessageObject(ID, new Data { team = t}),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.Platoon[(int)data.team].HaveBlackboard.RushLine = true;
			}
		}
	}
}
