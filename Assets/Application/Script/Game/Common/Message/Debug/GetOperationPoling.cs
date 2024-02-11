using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class CommonMessage {
		/// <summary>
		/// 操作情報取得
		/// </summary>
		public static class GetOperationPoling {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public OperationInfo info;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(OperationInfo info) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){info = info})
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(UserOperation op, MessageObject msg) {
				Data data = (Data)msg.Data;
				data.info.Init(op.PollingList);
			}
		}
	}
#endif
}
