using Project.Http.Mst;
using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class BattleMessage {
		/// <summary>
		/// アタック範囲表示
		/// </summary>
		public static class AttackAreaDisplay {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Power power;
				public Abreast abreast;
				public ACTION_PATTERN pattern;
				public bool flag;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Power p, Abreast a, ACTION_PATTERN pt, bool f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ power = p, abreast = a, pattern = pt, flag = f}),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DebugAttackArea area, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (area.Owner.Platoon.Index != data.power)
					return;

				if (area.Owner.Squad.Index != data.abreast)
					return;

				if (area.pattern != data.pattern)
					return;

				area.SetVisible(data.flag);
			}
		}
	}
#endif
}
