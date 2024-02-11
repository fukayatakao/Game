using Project.Lib;
using Project.Network;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// 分隊所属のリーダー変更
		/// </summary>
		public static class ChangeSquadLeader {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public int abreast;
				public LeaderData leader;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(int a, LeaderData leaderData) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { abreast = a, leader = leaderData }),
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

				//リーダーの変更
				var leaderData = OrganizeSituationData.I.AllocLeader(data.leader);
				//列のCharacterEntityを生成
				main.Platoon.ChangeLeaderAsync(data.abreast, leaderData, false, () => {
					main.Platoon.InitLocation(main.StageField.CenterDepth, main.StageField.TerritoryDepth, false);
					CharacterAssembly.I.Flush();
					main.UpdatePlatoonData();
				});
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
