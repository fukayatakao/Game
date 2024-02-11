using Project.Lib;


namespace Project.Game {
	//イベント受け取りように仮のダウンロード開始UIクラスを定義する
	public class DummyDownloadUI {
		MessageSystem.Receptor receptor_;


		public DummyDownloadUI() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DownloadEvent);
		}

		public void ShowDownloadSize(int size) {
			//本当はokボタンなどの操作を行う必要がある。まだUIなど作ってないので即次へ。
			CommonMessage.StartDownload.Broadcast();
		}

		public void Release() {
			MessageSystem.DestroyReceptor(receptor_);
		}
	}
}
