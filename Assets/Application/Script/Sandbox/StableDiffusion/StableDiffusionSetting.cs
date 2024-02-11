using System.Net;
using UnityEngine;


public static class StableDiffusionSetting {
	//StableDiffusionのURL
	public const string StableDiffusionServerURL = "http://127.0.0.1:7860";
	public const string ModelsAPI = "/sdapi/v1/sd-models";
	public const string TextToImageAPI = "/sdapi/v1/txt2img";
	public const string ImageToImageAPI = "/sdapi/v1/img2img";
	public const string OptionAPI = "/sdapi/v1/options";
	public const string ProgressAPI = "/sdapi/v1/progress";





	/// <summary>
	/// Update a generation progress bar
	/// </summary>
	public static void UpdateGenerationProgress() {
#if UNITY_EDITOR
		// Stable diffusion API url for Setting a model
		string url = StableDiffusionServerURL + ProgressAPI;

		float progress = 0;

		using (WebClient client = new WebClient()) {
			// Send the GET request
			string responseBody = client.DownloadString(url);

			// Deserialize the response to a class
			SDProgress sdp = JsonUtility.FromJson<SDProgress>(responseBody);
			progress = sdp.progress;
			Debug.Log(progress);
			//EditorUtility.DisplayProgressBar("Generation in progress", (progress * 100).ToString("F1") + "%", progress);
		}
#endif
	}

}

