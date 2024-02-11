using UnityEngine;
using Project.Lib;


namespace Project.Game {
	public static partial class OrganizeMessage {
		/// <summary>
		/// キャラの場所入れ替え
		/// </summary>
		public static class SwapCharacter {
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
			public static void Broadcast(CharacterEntity a, CharacterEntity t) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { actor = a, target = t }),
					(int)MessageGroup.UserEvent
				);
			}
			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(OrganizeMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				CharacterEntity actor = data.actor;
				CharacterEntity target = data.target;

				//所属グループの何番目にいるかを保存して自分を除名
				int actorGroupIndex = -1;
				SquadEntity actorGroup = actor.Squad;
				int targetGroupIndex = -1;
				SquadEntity targetGroup = target.Squad;


				for(int i = 0, max = actorGroup.Members.Count; i < max; i++) {
					if(actorGroup.Members[i] == actor) {
						actorGroupIndex = i;
						break;
					}
				}
				Debug.Assert(actorGroupIndex >= 0, "not found actor in group");

				for (int i = 0, max = targetGroup.Members.Count; i < max; i++) {
					if (targetGroup.Members[i] == target) {
						targetGroupIndex = i;
						break;
					}
				}
				Debug.Assert(targetGroupIndex >= 0, "not found target in group");

				actorGroup.ChangeCharacter(actorGroupIndex, target);
				targetGroup.ChangeCharacter(targetGroupIndex, actor);
				main.UpdatePlatoonData();
			}
		}
	}
}
