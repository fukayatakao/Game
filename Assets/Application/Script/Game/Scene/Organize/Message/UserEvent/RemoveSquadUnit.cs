using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// 分隊のユニット削除
		/// </summary>
		public static class RemoveSquadUnit {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int abreast;
				public int unitId;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int a, int id) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { abreast = a, unitId = id }),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				SquadEntity squad = main.Platoon.Squads[data.abreast];

				//列に所属してるキャラを予備に戻す
				OrganizeSituationData.I.FreeMember(squad.Members);

				squad.DestroyAllMembers();
				//リーダーもユニットもいなくなったら分隊削除
				if(squad.Leader == null && squad.Members == null) {
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
