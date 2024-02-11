using UnityEngine;
using Project.Lib;


namespace Project.Game {
	public static partial class CommonMessage {
		/// <summary>
		/// ダウンロードサイズ表示
		/// </summary>
		public static class ShowDownloadSize {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int size;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int s) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { size = s }),
					(int)MessageGroup.DownloadEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DummyDownloadUI ui, MessageObject msg) {
				Data data = (Data)msg.Data;

				Debug.Log("DummyDownloadUI download size = " + data.size);

				ui.ShowDownloadSize(data.size);
			}
		}
	}
}
