using UnityEngine;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class PerformanceWindow : DebugWindow {
        static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
        static readonly Vector2 size = new Vector2(0.2f, 0.4f);
        static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		const float IntervalTime = 0.5f;

		private float accumulateTime_ = 0f;	// FPS accumulated over the interval
		private int frames_ = 0;	// Frames drawn over the interval
		private float timer_;		// Left time for current_ interval
		private float fps_ = 0.0f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PerformanceWindow() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

        /// <summary>
        /// Window内部のUI表示
        /// </summary>
        protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			FitGUILayout.Label("FPS" + System.String.Format("{0:F2}", fps_), FitCommon.SMALL_FONT_SIZE);
			string mem = (UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)).ToString() + " MB";
			FitGUILayout.Label("MEM Alloc:", FitCommon.SMALL_FONT_SIZE);
			FitGUILayout.Label(mem, FitCommon.SMALL_FONT_SIZE);
			string mem2 = (UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)).ToString() + " MB";
			FitGUILayout.Label("Free :", FitCommon.SMALL_FONT_SIZE);
			FitGUILayout.Label(mem2, FitCommon.SMALL_FONT_SIZE);
		}

		//@note FPS計測は同じようなものがいっぱいあるのでそのうち誰か整理してほしい
		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			timer_ -= Time.deltaTime;
			accumulateTime_ += Time.timeScale / Time.deltaTime;
			++frames_;

			fps_ = accumulateTime_ / frames_;
			// Interval ended - update GUI text and start new interval
			if( timer_ <= 0f )
			{
				/*if( fps_ >= ( Application.targetFrameRate - 1f ) )
				{
					m_Color = Color.cyan;
				}
				else if( fps_ >= ( Application.targetFrameRate / 1.5f ) )
				{
					m_Color = Color.green;
				}
				else if( fps_ >= ( Application.targetFrameRate / 2f ) )
				{
					m_Color = Color.yellow;
				}
				else
				{
					m_Color = Color.red;
				}*/

				timer_ = IntervalTime;
				accumulateTime_ = 0f;
				frames_ = 0;
			}

		}
	}
#endif
}
