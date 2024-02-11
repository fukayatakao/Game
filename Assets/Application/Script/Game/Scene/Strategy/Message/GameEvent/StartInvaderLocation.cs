using Project.Lib;

namespace Project.Game {
	public static partial class StrategyMessage {
		/// <summary>
		/// 攻撃側の駒配置開始
		/// </summary>
		public static class StartInvaderLocation {
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
			private static void Recv(StrategyMain main, MessageObject msg) {
				main.ChangeState(StrategyMain.State.InvaderLocation);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(InvaderLocationMenu menu, MessageObject msg) {
				menu.Show();
			}
		}
	}
}