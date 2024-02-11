using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;


using UnityEngine;
using Project.Network;
#pragma warning disable 0649
public class StableDiffusionModel {
	/// <summary>
	/// SettingData structure that represents a Stable Diffusion model to help deserialize from JSON string.
	/// </summary>
	[Serializable]
	class Model {
		public string title;
		public string model_name;
		public string hash;
		public string sha256;
		public string filename;
		public string config;
	}


	//権限設定
	[Header("API Settings")]
	public bool useAuth = false;
	public string user;
	public string pass;

	[SerializeField]
	public List<string> modelNames;
	/// <summary>
	/// Stable Diffusionのmodelリストを取得
	/// </summary>
	public void ListModelsAsync() {
		// Stable diffusion API url for getting the models list
		string url = StableDiffusionSetting.StableDiffusionServerURL + StableDiffusionSetting.ModelsAPI;

		// Tell Stable Diffusion to use the specified model using an HTTP POST request
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.Method = "GET";

		// add auth-header to request
		if (useAuth) {
			//userとpassが設定されてないときはエラー
			Debug.Assert(!string.IsNullOrEmpty(user), "user is not set");
			Debug.Assert(!string.IsNullOrEmpty(pass), "pass is not set");

			httpWebRequest.PreAuthenticate = true;
			byte[] bytesToEncode = Encoding.UTF8.GetBytes(user + ":" + pass);
			string encodedCredentials = Convert.ToBase64String(bytesToEncode);
			httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
		}


		try {
			// Get the response of the server
			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
				string result = streamReader.ReadToEnd();
				// Deserialize the response to a class
				List<Model> ms = JsonList.FromJsonList<Model>(result);

				//モデル名だけを変数に入れる
				modelNames = new List<string>();
				foreach (Model m in ms)
					modelNames.Add(m.model_name);
			}
		} catch (System.Exception) {
			Debug.Log("Server needs and API key authentication. Please check your settings!");
		}
	}
}
