namespace Project.Game {
	public enum MessageGroup {
		All,
		SystemEvent,            //システム要求のイベント
		UserEvent,              //ユーザー操作によるイベント
		GameEvent,              //ゲーム中に発生したイベント（キャラの死亡や状態異常の発生など
		TouchControl,           //タッチ制御の切り替えイベント（キャラをタッチしてドラッグして移動させる操作に切り替えなど
		DownloadEvent,          //ダウンロードイベント（ダウンロード処理のUI,システム間メッセージ
		JournalEvent,           //記録用イベント
		DebugEvent,             //デバッグ専用イベント
		Max,
	}
}
