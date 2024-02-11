using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Text2Image {
	public string prompt;
	public string negativePrompt;
	public int steps = 90;
	public float cfgScale = 7;
	public int width = 512;
	public int height = 512;
	public long seed = -1;
	public string sampler_name = "Euler a";



	public async void GenerateAsync() {
		//@todo 生成に使うモデル指定
		//yield return sdc.SetModelAsync(modelsList[selectedModel]);

		// Generate the image
		HttpWebRequest httpWebRequest = null;
		try {
			// Make a HTTP POST request to the Stable Diffusion server
			httpWebRequest = (HttpWebRequest)WebRequest.Create(StableDiffusionSetting.StableDiffusionServerURL + StableDiffusionSetting.TextToImageAPI);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			// add auth-header to request
			/*if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals("")) {
				httpWebRequest.PreAuthenticate = true;
				byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
				string encodedCredentials = Convert.ToBase64String(bytesToEncode);
				httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
			}*/

			// Send the generation parameters along with the POST request
			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
				SDParamsInTxt2Img2222 sd = new SDParamsInTxt2Img2222();
				sd.prompt = prompt;
				sd.negative_prompt = negativePrompt;
				sd.steps = steps;
				sd.cfg_scale = cfgScale;
				sd.width = width;
				sd.height = height;
				sd.seed = seed;
				sd.tiling = false;
				sd.sampler_name = sampler_name;

				// Serialize the input parameters
				string json = JsonUtility.ToJson(sd);

				// Send to the server
				streamWriter.Write(json);
			}
		} catch (Exception e) {
			Debug.LogError(e.Message + "\n\n" + e.StackTrace);
		}

		// Read the output of generation
		if (httpWebRequest != null) {
			// Wait that the generation is complete before procedding
			Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();

			while (!webResponse.IsCompleted) {
				/*if (sdc.settings.useAuth)
					UpdateGenerationProgressWithAuth();
				else*/
				StableDiffusionSetting.UpdateGenerationProgress();

				await Task.Delay(500);
			}

			// Stream the result from the server
			var httpResponse = webResponse.Result;

			using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
				// Decode the response as a JSON string
				string result = streamReader.ReadToEnd();

				// Deserialize the JSON string into a data structure
				SDResponseTxt2Img json = JsonUtility.FromJson<SDResponseTxt2Img>(result);

				// If no image, there was probably an error so abort
				if (json.images == null || json.images.Length == 0) {
					Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

#if UNITY_EDITOR
					//EditorUtility.ClearProgressBar();
#endif
					//yield break;
					return;
				}

				// Decode the image from Base64 string into an array of bytes
				byte[] imageData = Convert.FromBase64String(json.images[0]);

				// Write it in the specified project output folder
				using (FileStream imageFile = new FileStream("aaa", FileMode.Create)) {
#if UNITY_EDITOR
					//AssetDatabase.StartAssetEditing();
#endif
					await imageFile.WriteAsync(imageData, 0, imageData.Length);
#if UNITY_EDITOR
					//AssetDatabase.StopAssetEditing();
					//AssetDatabase.SaveAssets();
#endif
				}

				try {
					// Read back the image into a texture
					if (File.Exists("aaa")) {
						Texture2D texture = new Texture2D(2, 2);
						texture.LoadImage(imageData);
						texture.Apply();

						//LoadIntoImage(texture);
					}

					// Read the generation info back (only seed should have changed, as the generation picked a particular seed)
					/*if (json.info != "") {
						SDParamsOutTxt2Img info = JsonUtility.FromJson<SDParamsOutTxt2Img>(json.info);

						// Read the seed that was used by Stable Diffusion to generate this result
						generatedSeed = info.seed;
					}*/
				} catch (Exception e) {
					Debug.LogError(e.Message + "\n\n" + e.StackTrace);
				}
			}
		}
#if UNITY_EDITOR
		//EditorUtility.ClearProgressBar();
#endif
	}
}
