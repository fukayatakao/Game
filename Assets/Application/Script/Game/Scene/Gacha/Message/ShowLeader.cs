using Project.Lib;
using Project.Network;
using UnityEngine;

namespace Project.Game {
	public static partial class GachaMessage {
		/// <summary>
		/// ガチャ結果キャラ表示
		/// </summary>
		public static class ShowLeader {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public LeaderData leader;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(LeaderData c) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { leader = c }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(GachaMain main, MessageObject msg) {
				Data data = (Data)msg.Data;
				string Portrait = "Character/Leader/leader_" + data.leader.id.ToString("000");
				CharacterPortraitUtil.LoadLeaderTexture(Portrait, (texture) => {
					main.Image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
				});

			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharaStatus menu, MessageObject msg) {
				Data data = (Data)msg.Data;

				//menu.Init(data.chara);
				menu.Hide();

			}
		}
	}
}
