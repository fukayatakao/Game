namespace Project.Game {
	//制御優先度
	public enum OperationPriority {
#if DEVELOP_BUILD
		DebugSelectControl,         //キャラ選択デバッグ用
#endif
		CharacterDrag,              //キャラクター制御
		CharacterMove,              //キャラクター移動制御

		NodeSelect,

		//デフォルトなので一番優先度を下げる
		CameraDefault = 1000,
		CameraEditorViewing,              //カメラのビュー操作中
		CameraViewing,              //カメラのビュー操作中
	}
}
