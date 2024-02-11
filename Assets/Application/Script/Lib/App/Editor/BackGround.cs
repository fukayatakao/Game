using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Project.Game {

	public static class BackGround {
		//追加するシンボル定義
		const string background = "RUN_BACKGROUND";
		//再生するシーン
		const string EditorScenePath = "Assets/Application/Scene/Game/Battle.unity";
		/// <summary>
		/// コマンドラインからの実行
		/// </summary>
		public static void Run() {
			Debug.Log("Kick Start");
			try {
				//コマンドライン引数から設定を取得
				string[] args = System.Environment.GetCommandLineArgs();
				for (int i = 0; i < args.Length; i++) {
					//任意引数は"定義":"値"の形で渡すというルールで運用
					string[] split = args[i].Split(":");
					if (split.Length < 2)
						continue;
					string key = split[0].ToLower();
					string value = split[1].ToLower();


					switch (key) {
					//倍速
					case "times":
						PlayerPrefs.SetInt(BattleMain.TimesKey, int.Parse(value));
						break;
					//試行回数
					case "loop":
						PlayerPrefs.SetInt(BattleMain.LoopKey, int.Parse(value));
						break;
					}
				}
			//例外が起きたときは実行せずに止める
			} catch (System.Exception e) {
				Debug.LogError(e);
				return;
			}

			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorScenePath);
			//シンボル定義の追加
			string add = ";" + background;
			string symbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);

			//すでに定義されている場合は追加しないようにする
			string[] defs = symbols.Split(';');
			for(int i = 0; i < defs.Length; i++) {
				if(defs[i] == background) {
					add = "";
					break;
				}
			}
			UnityEditor.PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, symbols + add);
			//シーンを再生
			EditorApplication.isPlaying = true;

		}

	}
}
