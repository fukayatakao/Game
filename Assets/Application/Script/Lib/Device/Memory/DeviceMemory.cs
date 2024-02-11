namespace Project.Lib {
#if DEVELOP_BUILD
	public class DeviceMemory {

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern long UseMemorySize();

		public static long GetMemorySize() {
			return (long)UseMemorySize();
		}


#elif UNITY_ANDROID
		static AndroidJavaObject activityManager = null;
		public static long GetMemorySize() {
			//activeManagerがない場合は取得する
			if(activityManager == null) {
				var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				var application = activity.Call<AndroidJavaObject>("getApplication");
				var context = activity.Call<AndroidJavaObject>("getApplicationContext");
				var staticContext = new AndroidJavaClass("android.content.Context");
				var service = staticContext.GetStatic<AndroidJavaObject>("ACTIVITY_SERVICE");
				activityManager = activity.Call<AndroidJavaObject>("getSystemService", service);
			}
			var process = System.Diagnostics.Process.GetCurrentProcess();
			var pidList = new int[] { process.Id };
			var memoryInfoList = activityManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", pidList);

			long total = 0;
			foreach (var memoryInfo in memoryInfoList) {
				total += memoryInfo.Call<int>("getTotalPss") * 1024;// kB単位なのでByte単位に直す
			}
			return total;
		}
#else
		public static long GetMemorySize() {
			return UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
		}
#endif
	}
#endif
}
