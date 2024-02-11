#if MANIPULATE_TIME
/// <summary>
/// Time操作クラス
/// </summary>
public static class Time
{
	enum State
	{
		Original,
		Default,
		Stop,
		Reverse,
	}

	const int TARGET_FRAME = 60;
	const float DELTA_TIME = 1f / TARGET_FRAME;
	//逆再生フラグ
	public static bool IsReverse { get { return reverse_; } }
	//停止フラグ
	public static bool IsStop { get { return stop_; } }
	//演出などでスロー再生する用にオリジナルモードを用意
	public static bool IsOriginal;

	private static bool step_;
	private static bool stop_;
	private static bool reverse_;
	private static float deltaTime_ = DELTA_TIME;

	//=========================================================================================
	//UnityEngine.Timeに存在するもの
	public static float deltaTime { get { return deltaTime_; } private set { deltaTime_ = value; } }
	private static float timeScale_ = 1f;
	public static float timeScale
	{
		get { return timeScale_; }
		set {
			timeScale_ = value;
			if (timeScale_ == 0f)
				stop_ = true;
			else
				stop_ = false;
			/*UnityEngine.Time.timeScale = timeScale_;*/
		}
	}

	private static float time_;
	public static float time { get { return time_; } }

	private static int frameCount_ = 0;

	//=========================================================================================
	//UnityEngine.TimeオリジナルをWrap
	public static float realtimeSinceStartup { get { return UnityEngine.Time.realtimeSinceStartup; } }
	public static float fixedDeltaTime { get { return UnityEngine.Time.fixedDeltaTime; } set { UnityEngine.Time.fixedDeltaTime = value; } }


	/// <summary>
	/// フレームレートを設定
	/// </summary>
	public static void Setup() {
		// 目標フレームレート
		UnityEngine.Application.targetFrameRate = TARGET_FRAME;

		stop_ = false;
		reverse_ = false;
	}
	public static void Reverse() {
		reverse_ = !reverse_;
	}
	public static void Stop() {
		UnityEngine.Debug.Log(stop_);
		stop_ = !stop_;
		UnityEngine.Debug.Log(stop_);
	}
	/// <summary>
	/// 記録が必要か
	/// </summary>
	public static bool IsRecord() {
		return (!stop_ && !reverse_) || (stop_ && step_ && !reverse_);
	}
	/// <summary>
	/// 再生が必要か
	/// </summary>
	public static bool IsReplay() {
		return (!stop_ && reverse_) || (stop_ && step_ && reverse_);
	}

	/// <summary>
	/// 時間経過
	/// </summary>
	public static void Execute() {
		if (stop_) {
			if (step_) {
				if (!reverse_) {
					ExecuteDefault();
				} else {
					ExecuteReverse();
				}
			} else {
				ExecuteStop();
			}
		} else {
			if (!reverse_) {
				ExecuteDefault();
			} else {
				ExecuteReverse();
			}

		}
	}

	private static void ExecuteReverse() {
		frameCount_--;

		timeScale = 0f;
		deltaTime = -DELTA_TIME;
		time_ = frameCount_ * DELTA_TIME;
	}

	private static void ExecuteDefault() {
		timeScale = 1f;
		frameCount_++;
		deltaTime = DELTA_TIME;
		time_ = frameCount_ * DELTA_TIME;
	}
	private static void ExecuteStop() {
		timeScale = 0f;
		deltaTime = 0f;
	}


#if DEVELOP_BUILD
	public static void DebugKey() {
		if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S)) {
			Time.Stop();
		}
		if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R))
		{
			Time.Reverse();
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space))
		{
			step_ = true;
		}
		else
		{
			step_ = false;
		}

	}
#endif
}
#endif
