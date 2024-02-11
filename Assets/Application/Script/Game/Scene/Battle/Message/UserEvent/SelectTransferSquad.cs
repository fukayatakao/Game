using Project.Lib;


namespace Project.Game {
    public static partial class BattleMessage {
        /// <summary>
        /// 分隊を選択した
        /// </summary>
        public static class SelectTransferSquad {
            //メッセージ種別のID
            private static int ID = -1;
            //送付するデータ内容
            private class Data {
				public SquadEntity squad;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(SquadEntity s) {
                MessageSystem.Broadcast(
                    new MessageObject(ID, new Data { squad = s }),
                    (int)MessageGroup.UserEvent
                );
            }

            /// <summary>
            /// メッセージを受信して処理
            /// </summary>
            private static void Recv(CharacterEntity entity, MessageObject msg) {
                Data data = (Data)msg.Data;
				if (data.squad != entity.Squad)
					return;
				entity.HaveOutline.SelectGroup();
			}
		}

		/// <summary>
		/// 分隊を選択した
		/// </summary>
		public static class UnSelectTransferSquad {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public SquadEntity squad;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(SquadEntity s) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { squad = s }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterEntity entity, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (data.squad != entity.Squad)
					return;
				entity.HaveOutline.UnSelect();
			}
		}
	}
}
