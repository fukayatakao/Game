using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class CommonMessage {
		/// <summary>
		/// デバッグ表示切替
		/// </summary>
		public static class DebugMenuShow {
			//メッセージ種別のID
			private static int ID = -1;

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, null),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TitleAlternativeMenu menu, MessageObject msg) {
				menu.Show();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownAlternativeMenu menu, MessageObject msg) {
				menu.Show();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleAlternativeMenu menu, MessageObject msg) {
				menu.Show();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeAlternativeMenu menu, MessageObject msg) {
				menu.Show();
			}
#if UNITY_EDITOR
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DummyAlternativeMenu menu, MessageObject msg) {
				menu.Show();
			}
#endif
		}
		/// <summary>
		/// デバッグ表示切替
		/// </summary>
		public static class DebugMenuHide {
			//メッセージ種別のID
			private static int ID = -1;

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, null),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TitleAlternativeMenu menu, MessageObject msg) {
				menu.Hide();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownAlternativeMenu menu, MessageObject msg) {
				menu.Hide();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleAlternativeMenu menu, MessageObject msg) {
				menu.Hide();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeAlternativeMenu menu, MessageObject msg) {
				menu.Hide();
			}
#if UNITY_EDITOR
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DummyAlternativeMenu menu, MessageObject msg) {
				menu.Hide();
			}
#endif
		}
	}
#endif
}
