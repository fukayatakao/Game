using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class GetUrlCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "common",
				Server = Command.Config.ServerRoot[ "root" ],
				UserAgent = Command.Config.UserAgent[ "sim" ],
				EncryptKey = Command.Config.DefaultEncryptKey,
				TimeoutTime = 30f,
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
		public async static Task<Command.Result> CreateTask(GetUrlRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(GetUrlRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<GetUrlResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class GetUrlRequest : Request
	{
		public GetUrlRequest(string check_code, int platform, int language){
			this.check_code = check_code;
			this.platform = platform;
			this.language = language;

		}
		//---- リクエスト変数定義 ----
		public string check_code; //クライアント側チェックコード
		public int platform; //プラットフォームID PlatformDef.cs
		public int language; //言語 LanguageDef

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class GetUrlResponse : Response
	{
		//---- レスポンス変数定義 ----
		public string asset_bundle_url; //
		public string asset_bundle_version; //
		public string base_data_url; //接続先URL
		public string notice_url; //告知URL
		public string server_id; //ユーザーID保存用
		public int is_need_version_up; //バージョンアップが必要フラグ

	}

}
