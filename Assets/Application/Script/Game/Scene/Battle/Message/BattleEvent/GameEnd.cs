using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// ゲーム終了
		/// </summary>
		public static class GameEnd {
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
#if DEVELOP_BUILD
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleEndMessage message, MessageObject msg) {
				message.Show();
			}
#endif
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				main.GameFinish();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleTimer timer, MessageObject msg) {
				timer.Stop();
			}
		}
	}
}
