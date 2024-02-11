using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
    /// <summary>
    /// キーボードを使ったデバッグ操作
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class OrganizeDebugKeyControl : MonoBehaviour{
		//ゲームメイン
		OrganizeMain main_;
		/// <summary>
		/// 生成処理
		/// </summary>
		public static void Create(OrganizeMain main) {
			GameObject obj = new GameObject("Debug");
			//デバッグ用キー操作
			obj.AddComponent<OrganizeDebugKeyControl>().Init(main);
		}
		/// <summary>
		/// 初期化
		/// </summary>
		private void Init(OrganizeMain main) {
            main_ = main;
        }
		/// <summary>
		/// 実行処理
		/// </summary>
		private void Update() {
			//終了処理テスト
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if(EntryPoint.FirstBoot == EntryPoint.BootScene.Town) {
					Application.Quit();
				} else {
					UnityEngine.SceneManagement.SceneManager.LoadScene("title");
				}
			}
		}

	}
#endif
}
