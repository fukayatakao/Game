using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// ゲーム開始
		/// </summary>
		public static class GameStart {
			//メッセージ種別のID
			private static int ID = -1;
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, null),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				main.GameStart();
				main.ChangeState(BattleMain.State.Main);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleTimer timer, MessageObject msg) {
				timer.GameStart();
			}
		}
	}
}
