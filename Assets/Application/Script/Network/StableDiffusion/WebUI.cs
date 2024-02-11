using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Project.Network;

namespace Project.StableDiffusion {
	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class Text2ImageRequest {
		public bool enable_hr = false;
		public float denoising_strength = 0;
		public int firstphase_width = 0;
		public int firstphase_height = 0;
		public float hr_scale = 2;
		public string hr_upscaler = "";
		public int hr_second_pass_steps = 0;
		public int hr_resize_x = 0;
		public int hr_resize_y = 0;
		public string prompt = "";
		public string[] styles = { "" };
		public long seed = -1;
		public long subseed = -1;
		public float subseed_strength = 0;
		public int seed_resize_from_h = -1;
		public int seed_resize_from_w = -1;
		public string sampler_name = "Euler a";
		public int batch_size = 1;
		public int n_iter = 1;
		public int steps = 50;
		public float cfg_scale = 7;
		public int width = 512;
		public int height = 512;
		public bool restore_faces = false;
		public bool tiling = false;
		public string negative_prompt = "";
		public float eta = 0;
		public float s_churn = 0;
		public float s_tmax = 0;
		public float s_tmin = 0;
		public float s_noise = 1;
		public bool override_settings_restore_afterwards = true;
		public string sampler_index = "Euler";

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class Text2ImageResponse {
		public string[] images;
		public SDParamsOutTxt2Img parameters;
		public string info;
	}


	public class WebUIConnection {
		const float TIMEOUT_SEC = 600f;
		const string ServerURL = "http://127.0.0.1:7860";
		const string ModelsAPI = "/sdapi/v1/sd-models";
		const string TextToImageAPI = "/sdapi/v1/txt2img";
		const string ImageToImageAPI = "/sdapi/v1/img2img";
		const string OptionAPI = "/sdapi/v1/options";
		const string ProgressAPI = "/sdapi/v1/progress";


		public static async void TextToImangeAsync(Text2ImageRequest request, System.Action<Text2ImageResponse> success = null, System.Action<System.Net.WebExceptionStatus> failure = null) {
			var result = await HttpRequest(ServerURL + TextToImageAPI, request, (json) => { return JsonUtility.FromJson<Text2ImageResponse>(json); }, TIMEOUT_SEC, true);
			if(result.Item2 == WebExceptionStatus.Success) {
				success(result.Item1);
			} else {
				failure(result.Item2);
			}
		}

		/// <summary>
		/// Update a generation progress bar
		/// </summary>
		public static float UpdateGenerationProgress() {
#if UNITY_EDITOR
			// Stable diffusion API url for Setting a model
			string url = ServerURL + ProgressAPI;


			using (WebClient client = new WebClient()) {
				// Send the GET request
				string responseBody = client.DownloadString(url);

				// Deserialize the response to a class
				SDProgress sdp = JsonUtility.FromJson<SDProgress>(responseBody);
				return sdp.progress;
			}
#else
			return 0f;
#endif
		}


		/// <summary>
		/// 通常の通信用
		/// </summary>
		private async static Task<(res, WebExceptionStatus)> HttpRequest<res, req>(string url, req request, System.Func<string, res> Parser, float timeoutSec = 10f, bool isPost=true) where res : new() {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = isPost ? "POST" : "GET";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);
				// 送るデータを書き込み
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
					string json = JsonUtility.ToJson(request);
					streamWriter.Write(json);
				}
				return httpWebRequest;
			};
			return await Web.Connection<res, req>(createHttp, Parser);
		}
	}





}
