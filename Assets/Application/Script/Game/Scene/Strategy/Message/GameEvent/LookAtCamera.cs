using Project.Lib;

namespace Project.Game {
	public static partial class StrategyMessage {
		/// <summary>
		/// 駒を注視するカメラに切り替え
		/// </summary>
		public static class LookAtCameraOn {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public PieceEntity entity;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(PieceEntity e) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { entity = e}),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				main.MainCamera.ChangeControlFollow(data.entity.CacheTrans);
			}
		}
		/// <summary>
		/// 駒を注視するカメラに切り替え
		/// </summary>
		public static class LookAtCameraOff {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int nodeId;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int n) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { nodeId = n }),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(StrategyMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				main.MainCamera.ChangeControlView(true);

				//SceneTransition.ChangeBattle();
				//main.haveOffenseLayout.
			}
		}
	}
}
