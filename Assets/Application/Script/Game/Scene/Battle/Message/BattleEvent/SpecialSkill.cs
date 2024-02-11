using Project.Lib;


namespace Project.Game {
	public static partial class BattleMessage {
		/// <summary>
		/// リーダースキル開始
		/// </summary>
		public static class StartSpecialSkill {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity leader;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity l) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { leader = l}),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.ChangeState(BattleMain.State.Special);
				for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
					if (CharacterAssembly.I.Current[i] != data.leader) {
						CharacterAssembly.I.Current[i].ExecuteEnable = false;
					}
				}
			}
		}
		/// <summary>
		/// リーダースキル終了
		/// </summary>
		public static class EndSpecialSkill {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity leader;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity l) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { leader = l}),
					(int)MessageGroup.GameEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				main.ChangeState(BattleMain.State.Main);
				for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
					if (CharacterAssembly.I.Current[i] != data.leader) {
						CharacterAssembly.I.Current[i].ExecuteEnable = true;
					}
				}
			}
		}
	}
}
