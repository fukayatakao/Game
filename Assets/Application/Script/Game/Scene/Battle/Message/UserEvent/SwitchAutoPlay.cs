using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// ゲーム開始
		/// </summary>
		public static class SwitchAutoPlay {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Power team;
				public bool disable;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power t, bool en) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { team = t, disable = en }),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(PlatoonEntity platoon, MessageObject msg) {
				Data data = (Data)msg.Data;
				if(platoon.Index == data.team) {
					platoon.HaveThink.Enable = !data.disable;
				}
			}
		}
	}
}
