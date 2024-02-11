using Project.Lib;


namespace Project.Game {
	public static partial class CommonMessage {
		/// <summary>
		/// ダウンロード開始
		/// </summary>
		public static class StartDownload {
			//メッセージ種別のID
			private static int ID = -1;
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, null),
					(int)MessageGroup.DownloadEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DownloadWait wait, MessageObject msg) {
				wait.UnLock();
			}
		}
	}
}
