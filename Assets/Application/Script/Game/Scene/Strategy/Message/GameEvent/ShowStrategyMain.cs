using Project.Lib;

namespace Project.Game {
	public static partial class StrategyMessage {
		/// <summary>
		/// メニュー表示
		/// </summary>
		public static class StartPieceMove {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { }),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMainMenu menu, MessageObject msg) {
				menu.Hide();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMain main, MessageObject msg) {
				main.ChangeState(StrategyMain.State.Move);
			}
		}
		/// <summary>
		/// メニュー表示
		/// </summary>
		public static class EndPieceMove {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { }),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMainMenu menu, MessageObject msg) {
				menu.Show();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMain main, MessageObject msg) {
				main.ChangeState(StrategyMain.State.Main);
			}
		}
	}
}
