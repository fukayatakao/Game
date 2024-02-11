using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Network {

	public static class Web {
		//レスポンスのチェック間隔(ms)
		private const int SLEEP_TIME = 15;
		/// <summary>
		/// 通常の通信用
		/// </summary>
		public async static Task<(Response, WebExceptionStatus)> HttpCommand(string url, Request request, System.Func<string, Response> Parser, float timeoutSec = 10f, int delay = 0) {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);
				// 送るデータを書き込み
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
					string json = JsonUtility.ToJson(request);
					streamWriter.Write(json);
				}
				return httpWebRequest;
			};
			return await Connection<Response, Request>(createHttp, Parser);
		}

		/// <summary>
		/// 通常の通信用
		/// </summary>
		public async static Task<(res, WebExceptionStatus)> HttpPostRequest<res, req>(string url, req request, System.Func<string, res> Parser, float timeoutSec = 10f, int delay = 0) where res : new() {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);
				// 送るデータを書き込み
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
					string json = JsonUtility.ToJson(request);
					streamWriter.Write(json);
				}
				return httpWebRequest;
			};
			return await Connection<res, req>(createHttp, Parser);
		}
		/// <summary>
		/// 通常の通信用
		/// </summary>
		public async static Task<(res, WebExceptionStatus)> HttpGetRequest<res, req>(string url, req request, System.Func<string, res> Parser, float timeoutSec = 10f, int delay = 0) where res : new() {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "GET";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);
				// 送るデータを書き込み
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
					string json = JsonUtility.ToJson(request);
					streamWriter.Write(json);
				}
				return httpWebRequest;
			};
			return await Connection<res, req>(createHttp, Parser);
		}
		/// <summary>
		/// 通常の通信用
		/// </summary>
		public async static Task<(res, WebExceptionStatus)> HttpChatGPT<res, req>(string url, req request, System.Func<string, res> Parser, float timeoutSec = 10f, int delay = 0) where res : new() {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "GET";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);

				// 送るデータを書き込み
				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
					string json = JsonUtility.ToJson(request);
					streamWriter.Write(json);
				}
				return httpWebRequest;
			};
			return await Connection<res, req>(createHttp, Parser);
		}


		/// <summary>
		/// 非同期通信処理
		/// </summary>
		public async static Task<(res, WebExceptionStatus)> Connection<res, req>(System.Func<HttpWebRequest> createHttpRequest, System.Func<string, res> Parser, int delay=0) where res : new(){
			//遅延時間待機
			await Task.Delay(delay);
			//アクセス時間の計測
			long accessTimer = System.DateTime.Now.Ticks;

			// Make a HTTP POST request to the Stable Diffusion server
			HttpWebRequest httpWebRequest = createHttpRequest();
			res response = new res();
			WebExceptionStatus status = WebExceptionStatus.Success;
			try {
				// Wait that the generation is complete before procedding
				Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();
				//通信中
				while (!webResponse.IsCompleted) {
					await Task.Delay(SLEEP_TIME);
					Debug.Log("connection...");
				}


				// リクエスト成功
				using (var streamReader = new StreamReader(webResponse.Result.GetResponseStream())) {
					// Decode the response as a JSON string
					string json = streamReader.ReadToEnd();

#if UNITY_EDITOR
					System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
					sw.Start();
#endif

					// JSONデータのパーサー
					response = Parser(json);

#if UNITY_EDITOR
					sw.Stop();
					Debug.Log($"<color=#7070cc>{httpWebRequest.RequestUri.AbsoluteUri} parse time is {(((double)sw.ElapsedTicks) / 10000):F3}ms</color>");
#endif

				}
				//ここで接続のインスタンスは消える
				httpWebRequest.Abort();

			} catch (WebException e) {
				Debug.LogError(e);

				status = e.Status;
			} catch (System.Exception e) {
				Debug.LogError(string.Format("{0}\n{1}", e, httpWebRequest.RequestUri.AbsoluteUri));

				status = WebExceptionStatus.UnknownError;
			}
			return (response, status);
		}
	}
}
