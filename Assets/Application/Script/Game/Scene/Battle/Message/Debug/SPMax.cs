using Project.Http.Mst;
using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class BattleMessage {
		/// <summary>
		/// SP最大にする
		/// </summary>
		public static class SPMax {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Power power;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power p) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ power = p }),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(PlatoonEntity platoon, MessageObject msg) {
				Data data = (Data)msg.Data;

				if (platoon.Index != data.power)
					return;

				platoon.HaveSpecialPoint.AddPoint(GameConst.Battle.SPECIAL_POINT_MAX);
			}
		}
	}
#endif
}
