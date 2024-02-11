using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class BattleMessage {
		/// <summary>
		/// アタック範囲表示
		/// </summary>
		public static class SearchAreaDisplay {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity entity;
				public bool isDraw;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity e, bool flag) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ entity = e, isDraw = flag}),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DebugSearchArea area, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (!area.IsOwn(data.entity))
					return;
				area.SetVisible(data.isDraw);
			}
		}
	}
#endif
}
