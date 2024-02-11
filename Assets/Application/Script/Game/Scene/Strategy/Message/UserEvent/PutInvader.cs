using Project.Lib;

namespace Project.Game {
	public static partial class StrategyMessage {
		/// <summary>
		/// 駒の配置
		/// </summary>
		public static class PutInvader {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int nodeId;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int id) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { nodeId = id }),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyBoard board, MessageObject msg) {
				Data data = (Data)msg.Data;
				board.PutInvader(data.nodeId);
			}
		}
	}
}
