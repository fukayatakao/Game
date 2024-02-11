using System.Collections.Generic;
using Project.Lib;
using Project.Network;

namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// キャラの選択
		/// </summary>
		public static class UpdateCorps {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast() {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data {}),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;


				List<PlatoonData> platoonData = OrganizeSituationData.I.platoons;
				List<LeaderData> leaders = OrganizeSituationData.I.leaders;
				List<CharacterData> characters = OrganizeSituationData.I.members;


				UpdateCorpsRequest request = new UpdateCorpsRequest(platoonData, leaders, characters);

				UpdateCorpsCmd.CreateAsync(request, (s) => { });
			}
		}
	}
}
