using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public static partial class CommonMessage {
		//バトルと編成で使うため共用に移動
		/// <summary>
		/// デバッグ表示切替
		/// </summary>
		public static class CommonDebugDraw {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public bool collision;
				public bool target;
				public bool line;
				public bool ai;
				public bool phase;
				public bool report;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(bool collision, bool target, bool line, bool ai, bool phase, bool report) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { collision = collision, target = target, line = line, ai = ai, phase = phase, report = report }),
					(int)MessageGroup.DebugEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DebugCollisionDraw draw, MessageObject msg) {
				Data data = (Data)msg.Data;
				draw.SetVisible(data.collision);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DebugTargetDraw draw, MessageObject msg) {
				Data data = (Data)msg.Data;
				draw.SetVisible(data.target);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(SquadDeployLine line, MessageObject msg) {
				Data data = (Data)msg.Data;
				line.SetVisible(data.line);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(DebugAIState text, MessageObject msg) {
				Data data = (Data)msg.Data;
				text.SetVisible(data.ai);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterPhaseIcon phaseIcon, MessageObject msg) {
				Data data = (Data)msg.Data;
				if (data.phase) {
					phaseIcon.Show();
				} else {
					phaseIcon.Hide();
				}
			}
#if UNITY_EDITOR
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(BattleReport report, MessageObject msg) {
				Data data = (Data)msg.Data;
				report.SetOutput(data.report);
			}
#endif
		}
	}
#endif
}
