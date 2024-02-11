using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class BattleMessage {
		/// <summary>
		/// アタック範囲表示
		/// </summary>
		public static class CharacterAI {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity entity;
				public bool disable;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity e, bool f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){ entity = e, disable = f}),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterEntity entity, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (data.entity != null && data.entity != entity)
					return;
				entity.HaveThink.Enable = !data.disable;
				entity.ChangeState(data.disable ? CharacterState.State.None : CharacterState.State.Idle );
			}
		}
	}
#endif
}
