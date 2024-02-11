using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// ダウンロードの待機を行う
	/// </summary>
	public class DownloadWait {
		DummyDownloadUI ui = new DummyDownloadUI();

		bool wait_;
		MessageSystem.Receptor receptor_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DownloadWait() {
			//メッセージ受取用のインスタンスを作る
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DownloadEvent);
		}
		/// <summary>
		/// 待機開始
		/// </summary>
		public void Lock() {
			wait_ = true;
		}

		/// <summary>
		/// 待機終了
		/// </summary>
		public void UnLock() {
			wait_ = false;
		}

		/// <summary>
		/// 待機中か
		/// </summary>
		public bool IsWait() {
			return wait_;
		}
		/// <summary>
		/// インスタンスの後始末
		/// </summary>
		public void Release() {
			MessageSystem.DestroyReceptor(receptor_);
			ui.Release();
		}


	}


}
