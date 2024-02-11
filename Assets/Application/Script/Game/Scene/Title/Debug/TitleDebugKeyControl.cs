using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
    /// <summary>
    /// キーボードを使ったデバッグ操作
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class TitleDebugKeyControl : MonoBehaviour{
        /// <summary>
        /// 実行処理
        /// </summary>
        public void Update() {
			//終了処理テスト
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
		}

	}
#endif
}
