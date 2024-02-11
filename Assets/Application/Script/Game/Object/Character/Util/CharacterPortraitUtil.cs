using UnityEngine;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Game {
	public static class CharacterPortraitUtil {
		//@todo ここのキャッシュは後でどうにかする
		static Dictionary<string, Texture2D> characterTextureDict_ = new Dictionary<string, Texture2D>();
		/// <summary>
		/// BaseData取得のオフライン用関数
		/// </summary>
		public static string SaveTexture(byte[] imageData) {
			int count = 0;
			//既にある場合はカウンターをずらす
			while (PlayerPrefsUtil.HasKey(GetTextureKey(count))) {
				count++;
			}

			string key = GetTextureKey(count);
			//jsonを圧縮してBase64変換した結果をレスポンスで返す
			byte[] array = GZipUtil.compress(imageData);
			string base64Text = System.Convert.ToBase64String(array);
			PlayerPrefsUtil.SetString(key, base64Text);

			return key;
		}

		private static string GetTextureKey(int count) {
			return PrefsUtilKey.CharacterPortrait + "_" + count.ToString("0000");
		}

		/// <summary>
		/// キャラ画像をロードする(非オリジナル
		/// </summary>
		public static Texture2D LoadTextureFromResource(string portrait) {
			if (characterTextureDict_.ContainsKey(portrait)) {
				return characterTextureDict_[portrait];
			}
			string icon_path = "Character/Icon/" + portrait;
			Texture2D texture = Resources.Load<Texture2D>(icon_path);
			characterTextureDict_[portrait] = texture;
			return texture;
		}
		/// <summary>
		/// キャラ画像をロードする(オリジナル
		/// </summary>
		public static Texture2D LoadTextureFromPrefs(string portrait) {
			string base64Text = PlayerPrefsUtil.GetString(portrait);
			byte[] binary = System.Convert.FromBase64String(base64Text);

			var ret = GZipUtil.decompress2(binary);
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(ret);


			characterTextureDict_[portrait] = texture;
			return texture;
		}


		/// <summary>
		/// キャラ画像をロードする
		/// </summary>
		public static async void LoadLeaderTexture(string portrait, System.Action<Texture2D> action) {
			if (!characterTextureDict_.ContainsKey(portrait)) {
				var asset = await AddressableAssist.LoadAssetAsync(portrait);
				characterTextureDict_[portrait] = asset as Texture2D;
			}

			action(characterTextureDict_[portrait]);
		}


	}
}
