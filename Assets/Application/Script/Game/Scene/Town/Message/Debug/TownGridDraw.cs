using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class TownMessage {
		/// <summary>
		/// デバッグ表示切替
		/// </summary>
		public static class TownGridDraw {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public bool isDraw;
				public int tileMax;
				public int width;
				public int depth;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(bool draw, int tile, int w, int d) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { isDraw = draw, tileMax = tile, width = w, depth = d}),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownGridMap grid, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (data.isDraw) {
					grid.CreateDraw(data.tileMax, data.width, data.depth);
				}else {
					grid.DestroyDraw();
				}

			}
		}
	}
#endif
}
