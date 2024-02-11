using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// 分隊所属のリーダー削除
		/// </summary>
		public static class RemoveSquadLeader {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int abreast;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int a) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { abreast = a }),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				//列に所属してるリーダーを戻す
				SquadEntity squad = main.Platoon.Squads[data.abreast];
				OrganizeSituationData.I.FreeLeader(squad.Leader);

				//リーダーを削除
				squad.DestroyLeader();
				//リーダーもユニットもいなくなったら分隊削除
				if (squad.Leader == null && squad.Members == null) {
					main.Platoon.DestroySquad(data.abreast);
				}
				main.Platoon.InitLocation(main.StageField.CenterDepth, main.StageField.TerritoryDepth, false);
				main.UpdatePlatoonData();
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharacterSelectControl control, MessageObject msg) {
				control.UnSelectLastTarget();
			}
		}
	}
}
