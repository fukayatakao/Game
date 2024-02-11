using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class RegistAccountCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "RegistAccount.do",
				Server = Command.Config.ServerRoot[ "sim" ],
				UserAgent = Command.Config.UserAgent[ "sim" ],
				EncryptKey = Command.Config.DefaultEncryptKey,
				TimeoutTime = 10f,
				RetryCount = CommandSetting.DEFAULT_RETRY_COUNT,
				IsPostMethod = false,
				IsOffline = true,

#if USE_OFFLINE
				OfflineFunc = OfflineFunc,
#endif
				Parser = Parse,
			};
			return setting;
		}
		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static Task<Command.Result> CreateTask(RegistAccountRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(RegistAccountRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<RegistAccountResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class RegistAccountRequest : Request
	{
		public RegistAccountRequest(string device, int platform, int language){
			this.device = device;
			this.platform = platform;
			this.language = language;

		}
		//---- リクエスト変数定義 ----
		public string device; //デバイス名
		public int platform; //プラットフォーム PlatformDef
		public int language; //言語 LanguageDef

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class RegistAccountResponse : Response
	{
		//---- レスポンス変数定義 ----
		public string uuid; //UUID
		public string account_id; //アカウントID
		public string transfer_id; //引継ぎID
		public int user_id; //ユーザーID

	}

}
