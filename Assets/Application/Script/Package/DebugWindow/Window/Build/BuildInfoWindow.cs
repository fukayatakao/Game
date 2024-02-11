using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class BuildInfoWindow : DebugWindow {
		static readonly Rect DefaultWindowRect = FitCommon.CalcRect(new Vector2(0.01f, 0.01f), new Vector2(0.3f, 0.2f), FitCommon.Alignment.UpperRight);

		//ビルド情報が存在するか
		bool enableInfo_;

		string version_;
		string server_;
		string revision_;
		string hash_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BuildInfoWindow() {
			base.Init(DefaultWindowRect);

			TextAsset SettingText = Resources.Load("app_build_setting") as TextAsset;

			//設定情報が取れない場合は表示しないようにする
			enableInfo_ = SettingText != null;
			if (!enableInfo_)
				return;

			string[] info = SettingText.text.Split('\n');
			for (int i = 0; i < info.Length; i++) {
				string[] keyword = info[i].Split('=');
				//書式が想定したものではない行は無視
				if (keyword.Length < 2) {
					continue;
				}
				switch (keyword[0].Trim().ToUpper()) {
				case "VERSION":
					version_ = keyword[1].Trim();
					break;
				case "SERVER":
					server_ = keyword[1].Trim();
					break;
				case "REVISION":
					revision_ = keyword[1].Trim();
					break;
				case "HASH":
					hash_ = keyword[1].Trim();
					break;
				}
			}
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			if (!enableInfo_)
				return;
			FitGUILayout.Label("Version:" + version_);
			FitGUILayout.Label("Revision:" + revision_);
			FitGUILayout.Label("Hash:" + hash_);
			FitGUILayout.Label("Server:" + server_);
		}

	}
#endif
}
