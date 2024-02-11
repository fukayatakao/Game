using Project.Lib;
using Project.Network;

namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// メンバーの中身変更
		/// </summary>
		public static class ChangePersonal {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterEntity entity;
				public CharacterData inputCharacterData;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterEntity e, CharacterData i) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { entity = e, inputCharacterData = i }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				CharacterData change = data.entity.ChangePersonal(data.inputCharacterData);
				OrganizeSituationData.I.members.Remove(data.inputCharacterData);
				//無名キャラでなかったら予備に戻す
				if (change.id >= 0) {
					OrganizeSituationData.I.members.Add(change);
				}
				CharacterTask.BuildChangeEntry(data.entity);
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
