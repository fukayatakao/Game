using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;

namespace Project.Lib {

    public static class ProjectBuilder {

		//デバッグ用のシンボル定義
		private static readonly List<string> DEVELOP_SYMBOL = new List<string>() { "DEVELOP_BUILD", "TMP_ACTIVE_UNITY_EDITOR", "OFFLINE" };
		//リリース用のシンボル定義
		private static readonly List<string> RELEASE_SYMBOL = new List<string>();

		private static char[] separate = new char[]{':' };

        private static string fileName_ = "application";
        private static BuildTarget platform_ = BuildTarget.StandaloneWindows;
		private static List<string> defineSymbol_ = DEVELOP_SYMBOL;
        private static string defaultServer_ = "";

        //C:\Unity\Unity2019.2\Editor\Unity.exe -batchmode  -quit -logFile .\Logs\build.log -executeMethod Project.Lib.ProjectBuilder.Build name:test platform:android
        /// <summary>
        /// ROMビルド関数
        /// </summary>
        /// <remarks>
        /// バッチモードでコマンドラインから呼び出される
        /// </remarks>
        [MenuItem("Editor/Application/Build")]
        public static void Build() {
            try {
                string[] args = System.Environment.GetCommandLineArgs();
                for(int i = 0; i < args.Length; i++) {
                    string[] split = args[i].Split(separate);
                    if (split.Length < 2)
                        continue;
                    string key = split[0].ToLower();
                    string value = split[1].ToLower();


                    switch (key) {
                    case "name":
                        fileName_ = value;
                        break;
                    case "build":
                        if(value == "develop") {
                            defineSymbol_ = DEVELOP_SYMBOL;
                        }else if (value.ToLower() == "release") {
                            defineSymbol_ = RELEASE_SYMBOL;
                            PlayerSettings.usePlayerLog = false;
                        }
                        break;
                    case "platform":
                        if (value == "android") {
                            platform_ = BuildTarget.Android;
                        } else if (value == "ios") {
                            platform_ = BuildTarget.iOS;
                        } else if(value == "windows") {
							platform_ = BuildTarget.StandaloneWindows;
						} else {
                            //プラットフォームの設定が正しくない場合はエラー
							throw new System.Exception("build platform error");
                        }
                        break;
                    }
                }
            //例外が起きたときはビルドせずに止める
            } catch (System.Exception e) {
                Debug.LogError(e);
                return;
            }
			//Addressableのプロファイラは無効にする
			ProjectConfigData.PostProfilerEvents = false;

			string outName = "";
			if (platform_ == BuildTarget.Android) {
				outName = fileName_ + ".apk";
				string symbols = CreateSymbolString(UnityEditor.PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android), defineSymbol_);
				symbols += defaultServer_;
				UnityEditor.PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, symbols);

				Debug.Log("symbols = " + symbols);
			} else if ((platform_ == BuildTarget.iOS)) {
				outName = fileName_;
				string symbols = CreateSymbolString(UnityEditor.PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.iOS), defineSymbol_);
				symbols += defaultServer_;
				UnityEditor.PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, symbols);

				Debug.Log("symbols = " + symbols);
			} else if ((platform_ == BuildTarget.StandaloneWindows)) {
				outName = fileName_;
				string symbols = CreateSymbolString(UnityEditor.PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone), defineSymbol_);
				symbols += defaultServer_;
				UnityEditor.PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, symbols);

				Debug.Log("symbols = " + symbols);
			} else {
				Debug.LogError("Not found platform");
				return;
			}

			string[] scene = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            BuildPipeline.BuildPlayer(scene, outName, platform_, BuildOptions.None);

        }
		/// <summary>
		/// 重複するシンボルは排除して追加する
		/// </summary>
		private static string CreateSymbolString(string defineSymbols, List<string> defs) {
			string[] symbol = defineSymbols.Split(';');
			for (int i = 0; i < defs.Count; i++) {
				for (int j = 0; j < symbol.Length; j++) {
					if (defs[i] == symbol[j]) {
						defs.RemoveAt(i);
						i--;
						break;
					}
				}
			}

			string result = "";
			for (int i = 0; i < symbol.Length; i++) {
				result += symbol[i] + ";";
			}
			for (int i = 0; i < defs.Count; i++) {
				result += defs[i] + ";";
			}
			return result;
		}

	}
}
