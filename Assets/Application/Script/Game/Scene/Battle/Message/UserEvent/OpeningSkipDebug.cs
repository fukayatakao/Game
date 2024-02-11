using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// オープニングスキップ
		/// </summary>
		public static class OpeningSkipDebug {
			//メッセージ種別のID
			private static int ID = -1;

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, null),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				main.ChangeState(BattleMain.State.Ready);
			}
		}
	}
}
