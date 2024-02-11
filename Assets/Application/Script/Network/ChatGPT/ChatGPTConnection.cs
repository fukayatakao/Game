using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Project.Network;

namespace Project.ChatGPT {
	[Serializable]
	public class ChatGPTMessage {
		public string role;
		public string content;
	}

	//ChatGPT APIにRequestを送るためのJSON用クラス
	[Serializable]
	public class ChatGPTRequest {
		public string model;
		public List<ChatGPTMessage> messages;
	}

	//ChatGPT APIからのResponseを受け取るためのクラス
	[System.Serializable]
	public class ChatGPTResponse {
		public string id;
		public string @object;
		public int created;
		public Choice[] choices;
		public Usage usage;

		[System.Serializable]
		public class Choice {
			public int index;
			public ChatGPTMessage message;
			public string finish_reason;
		}

		[System.Serializable]
		public class Usage {
			public int prompt_tokens;
			public int completion_tokens;
			public int total_tokens;
		}
	}
	public static class ChatGPTConnection {
		private const string ChatGptUrl = "https://api.openai.com/v1/chat/completions";
		private const string ApiKey = "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";


		public static async void TalkAsync(ChatGPTRequest request, System.Action<ChatGPTResponse> success = null, System.Action<System.Net.WebExceptionStatus> failure = null) {
			var result = await HttpRequest(ChatGptUrl, request, (json) => { return JsonUtility.FromJson<ChatGPTResponse>(json); }, 600f);
			if (result.Item2 == WebExceptionStatus.Success) {
				success(result.Item1);
			} else {
				failure(result.Item2);
			}
		}


		/// <summary>
		/// 通常の通信用
		/// </summary>
		private async static Task<(res, WebExceptionStatus)> HttpRequest<res, req>(string url, req request, System.Func<string, res> Parser, float timeoutSec = 10f) where res : new() {
			System.Func<HttpWebRequest> createHttp = () => {
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";
				httpWebRequest.Timeout = (int)(timeoutSec * 1000);
				httpWebRequest.Headers.Add("Authorization", "Bearer " + ApiKey);


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


