using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class TaskSample2 : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {

		Tests("parallel1", 5000, true);
		Tests("parallel2", 5000, true);
		Tests("parallel3", 5000, true);
		Tests("parallel4", 5000, true);

		Task1();
		Task2();
		Task3();
	}

	// Update is called once per frame
	void Update() {

	}

	async void Tests(string log, int delay, bool isParallel) {
		await CorrespondAsync(log, delay, isParallel);

	}


	async void Task1() {
		await CorrespondAsync("task1", 0, false);
		await CorrespondAsync("task2", 5000, false);

	}
	async void Task2() {
		Task<int> task;
		task =  CorrespondAsync("task3", 5000, false);
		await task;

		Debug.Log("finish");
	}

	async void Task3() {
		Task<int> task;
		task = CorrespondAsync("task4", 5000, false);
		await task;

		Debug.Log("finish2");
	}


	//直列実行用のカレントTask
	private static Task currentCmd = Task.CompletedTask;
	//同時最大数制御用
	private SemaphoreSlim semaphore_ = new SemaphoreSlim(2);

	/// <summary>
	/// 非同期通信処理
	/// </summary>
	public async Task<int> CorrespondAsync(string log, int delay, bool isParallel) {
		Task<int> correspond;
		//並列実行
		if (isParallel) {
			//ロックを取得
			await semaphore_.WaitAsync();
			try {
				correspond = CorrespondAsyncImpl(log, delay);
				await correspond;
			} finally {
				//ロック解放
				semaphore_.Release();
			}
			//直列実行
		} else {
			correspond = currentCmd.ContinueWith(a => CorrespondAsyncImpl(log, delay)).Unwrap();
			currentCmd = correspond;
			await correspond;
		}

		Debug.Log(log + ":" + correspond.Result);
		return correspond.Result;
	}
	/// <summary>
	/// 非同期通信処理
	/// </summary>
	public async Task<int> CorrespondAsyncImpl(string log, int delay) {
		await Task.Delay(delay);
		return (int)(System.DateTime.Now.Ticks / 10000000);
	}
}
