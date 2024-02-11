using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class LotteryOriginalCmd
	{
		private static CommandSetting Setting() {
			var setting = new CommandSetting() {
				Api = "LotteryOriginal.do",
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
		public async static Task<Command.Result> CreateTask(LotteryOriginalRequest request) {
			return await CommandUtil.CreateTask(request, Setting());
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static void CreateAsync(LotteryOriginalRequest request, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
			await CommandUtil.CreateAsync(request, Setting(), success, failure);
		}

#if USE_OFFLINE
		public static System.Func<Request, Response> OfflineFunc = Command.Offline;
#endif
		/// <summary>
		/// Json解析
		/// </summary>
		private static Response Parse( string __text ) { return JsonUtility.FromJson<LotteryOriginalResponse>(__text); }

	}


	/// <summary>
	/// リクエストデータクラス
	/// </summary>
	public class LotteryOriginalRequest : Request
	{
		public LotteryOriginalRequest(int species, string portraitKey){
			this.species = species;
			this.portraitKey = portraitKey;

		}
		//---- リクエスト変数定義 ----
		public int species; //指定種族
		public string portraitKey; //ポートレート画像の取得キー

	}

	/// <summary>
	/// レスポンスデータクラス
	/// </summary>
	public class LotteryOriginalResponse : Response
	{
		//---- レスポンス変数定義 ----
		public CharacterData characterData; //キャラ個体データ

	}

}
