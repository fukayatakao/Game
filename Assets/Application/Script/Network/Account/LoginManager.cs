using Project.Network;
using Project.Mst;
using Project.Server;

namespace Project.Account {
	public static class LoginManager {

		public class AccountData {
			public string account;
			public string secure_id;
			public string uuid;
		}
		static AccountData accountData_ = new AccountData();

		const int platform = 0;
		const int language = 0;

		/// <summary>
		/// サーバにログインを試みる
		/// </summary>
		public static void Login(System.Action callback) {
			//成功したらBaseDataの更新をした後にcallbackを呼び出し、失敗したら新規ユーザー登録(仮)してBaseDataの更新->callback呼び出し
			LoginCmd.CreateAsync(new LoginRequest(accountData_.account, accountData_.secure_id, "", accountData_.uuid, platform, language, 0), (res)=> { LoginSuccess(res, callback); }, (res, status) => { LoginFailure(res, status, callback); });
		}
		/// <summary>
		/// ログインできたらBaseData更新
		/// </summary>
		public static void LoginSuccess(Response res, System.Action callback) {
			BaseDataManager.Refresh(()=>
			{
#if USE_OFFLINE
				//ダミーサーバ内でBaseData使っているためここで処理
				DummyServer.Login();
#endif
				callback();
			});
		}
		/// <summary>
		/// ログイン失敗
		/// </summary>
		public static void LoginFailure(Response res, System.Net.WebExceptionStatus status, System.Action callback) {
			//@note ダミーサーバの間はそのまま新規ユーザー作成
			RegisterAccount(callback);
		}
		/// <summary>
		/// 新規ユーザー登録
		/// </summary>
		public static void RegisterAccount(System.Action callback) {
			//@todo まだからっぽ
			RegistAccountCmd.CreateAsync(new RegistAccountRequest("", platform, language), (res) => { LoginSuccess(res, callback); }, (res, status) => { LoginFailure(res, status, callback); });
		}


	}
}
