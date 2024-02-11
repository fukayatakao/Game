using UnityEngine;

public class StableDiffusionTest : MonoBehaviour
{
	/*public class Test {
		public int[] array;
	}
	Text2Image text2image = new Text2Image();
	StableDiffusionModel Setting = new StableDiffusionModel();
	public UnityEngine.UI.Image image;
	// Start is called before the first frame update
	void Start()
    {
		bool wait = true;
		//直接シーンしていで起動された場合の処理
		EntryPoint.DirectBoot(EntryPoint.BootScene.Battle, () => { BattleMainCmd.CreateAsync(new BattleMainRequest(), (res)=>{ wait = false; }); });
		while (wait) {

		}

		//仮テクスチャ
		string icon_path = "Character/Icon/chara_0000";
		Texture2D texture = Resources.Load<Texture2D>(icon_path);
		image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		Debug.Log(image.sprite.name);


		//Setting.ListModelsAsync();
		//text2image.prompt = "a girl";
		//text2image.GenerateAsync();

		Text2ImageCmd.CreateAsync(new Text2ImageRequest() {
			prompt = "a girl",
		}, (res) => {
			Text2ImageResponse response = (Text2ImageResponse)res;
			// Decode the image from Base64 string into an array of bytes
			byte[] imageData = Convert.FromBase64String(response.images[0]);

			// Write it in the specified project output folder
			using (System.IO.FileStream imageFile = new System.IO.FileStream("abc", System.IO.FileMode.Create)) {
#if UNITY_EDITOR
				//AssetDatabase.StartAssetEditing();
#endif
				imageFile.Write(imageData, 0, imageData.Length);
				Texture2D texture = new Texture2D(1,1);
				texture.LoadImage(imageData);
				image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

#if UNITY_EDITOR
				//AssetDatabase.StopAssetEditing();
				//AssetDatabase.SaveAssets();
#endif
			}



		});

	}

    // Update is called once per frame
    void Update()
    {

    }*/
}
