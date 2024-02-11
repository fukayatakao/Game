using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class DeviceMemoryWindow : DebugWindow {
		static readonly Rect DefaultWindowRect = FitCommon.CalcRect(new Vector2(0.01f, 0.01f), new Vector2(0.3f, 0.2f), FitCommon.Alignment.UpperRight);

		long useSize_;
		float time_ = 0f;
		const float INTERVAL_TIME = 3f;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DeviceMemoryWindow() {
			base.Init(DefaultWindowRect);
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			//メモリ取得は重いので一定時間ごとに行う
			time_ -= UnityEngine.Time.unscaledDeltaTime;
			if (time_ < 0f) {
				//byte→MB変換してキャッシュ
				useSize_ = DeviceMemory.GetMemorySize() / (1024 * 1024);
				time_ = INTERVAL_TIME;
			}

			FitGUILayout.Label("Total:" + SystemInfo.systemMemorySize + "MB");
			FitGUILayout.Label("Use:" + useSize_ + "MB");
		}

	}
#endif
}
