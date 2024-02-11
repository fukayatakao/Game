using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class BattleMessage {
		/// <summary>
		/// 攻撃の結果をレポート
		/// </summary>
		public static class AttackReport {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity actor;
				public CharacterEntity target;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity actor, CharacterEntity target) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data() { actor = actor, target = target}),
					(int)MessageGroup.DebugEvent
				);
			}
		}
	}
#endif
}
