using Project.Lib;
using Project.Network;
using UnityEngine;

namespace Project.Game {
	public static partial class GachaMessage {
		/// <summary>
		/// ガチャ結果キャラ表示
		/// </summary>
		public static class ShowCharacter {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public CharacterData chara;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(CharacterData c) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { chara = c }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(GachaMain main, MessageObject msg) {
				Data data = (Data)msg.Data;

				Texture2D texture = data.chara.generate ? CharacterPortraitUtil.LoadTextureFromPrefs(data.chara.portrait) : CharacterPortraitUtil.LoadTextureFromResource(data.chara.portrait);

				main.Image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(CharaStatus menu, MessageObject msg) {
				Data data = (Data)msg.Data;

				menu.Init(data.chara);
				menu.Show();

			}
		}
	}
}
