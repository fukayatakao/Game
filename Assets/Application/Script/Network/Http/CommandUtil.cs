using System.Threading.Tasks;


namespace Project.Network
{
	/// <summary>
	/// 通信処理クラス
	/// </summary>
	public static class CommandUtil {
		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static Task<Command.Result> CreateTask(Request request, CommandSetting setting) {
#if USE_OFFLINE
			if (setting.IsOffline) {
				return new Command.Result() { Response = setting.OfflineFunc(request), ExceptionStatus = System.Net.WebExceptionStatus.Success };
			}
#endif
			return await new Command().CorrespondAsync(request, setting);
		}

		/// <summary>
		/// 非同期通信実行
		/// </summary>
		public async static Task CreateAsync(Request request, CommandSetting setting, System.Action<Response> success = null, System.Action<Response, System.Net.WebExceptionStatus> failure = null) {
#if USE_OFFLINE
			if (setting.IsOffline) {
				Response res = setting.OfflineFunc(request);
				if (success != null) {
					success(res);
				}
				return;
			}
#endif
			Command.Result result = await new Command().CorrespondAsync(request, setting);
			result.ExecCallback(success, failure);
		}
/*#if DEVELOP_BUILD
		/// <summary>
		/// 同期通信実行
		/// </summary>
		/// <remarks>
		/// テスト用の同期通信
		/// </remarks>
		public static Command.Result Create(Request request, CommandSetting Setting) {
#if USE_OFFLINE
			if (Setting.IsOffline) {
				return new Command.Result() { Response = Setting.OfflineFunc(request), ExceptionStatus = System.Net.WebExceptionStatus.Success };
			}
#endif
			return new Command().CorrespondSync(request, Setting);
		}

		/// <summary>
		/// 同期通信実行
		/// </summary>
		/// <remarks>
		/// テスト用の同期通信
		/// </remarks>
		public static void CreateSync(Request request, CommandSetting Setting, System.Action<Response> success, System.Action<Response, System.Net.WebExceptionStatus> failure) {
#if USE_OFFLINE
			if (Setting.IsOffline) {
				success(Setting.OfflineFunc(request));
				return;
			}
#endif
			Command cmd = new Command(request, Setting);
			Command.Result result = cmd.CorrespondSync(request);
			result.ExecCallback(success, failure);
		}
#endif

#if USE_OFFLINE
		/// <summary>
		/// オフライン処理
		/// </summary>
		public static Response Offline(Request req) {
			Debug.LogError("not found offline function:" + req.GetType());
			return new ResponseEmpty();
		}
#endif*/
	}

}
