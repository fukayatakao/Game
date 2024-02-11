using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// 分隊のユニット追加
		/// </summary>
		public static class AddSquadUnit {
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

				//新規列追加
				if (data.abreast >= main.Platoon.Squads.Count) {
					main.Platoon.CreateSquadAsync(data.abreast, data.unitId, null, false, (squad) => {
						//ライン制御を初期化
						squad.InitLine(main.StageField.Width, main.Platoon.TowardSign);

						main.Platoon.InitLocation(main.StageField.CenterDepth, main.StageField.TerritoryDepth, false);
						CharacterAssembly.I.Flush();
						SquadAssembly.I.Flush();
						main.UpdatePlatoonData();
					});
				} else {
					main.Platoon.ChangeUnitAsync(data.abreast, data.unitId, false, () => {
						main.Platoon.InitLocation(main.StageField.CenterDepth, main.StageField.TerritoryDepth, false);
						CharacterAssembly.I.Flush();
						main.UpdatePlatoonData();
					});

				}
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
