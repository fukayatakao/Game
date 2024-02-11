using Project.Lib;

namespace Project.Game {
	public static partial class StrategyMessage {
		/// <summary>
		/// 攻撃側の駒移動開始
		/// </summary>
		public static class MovePiece {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public StrategyBoard board;
				public Geopolitics.Way way;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(StrategyBoard b, Geopolitics.Way w) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { board = b, way = w}),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StateStrategyMove state, MessageObject msg) {
				Data data = (Data)msg.Data;
				state.BuildTask(data.board, data.way);
			}
		}
	}
}
