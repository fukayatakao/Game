using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;
using Project.Lib;

public class TaskCancelSample : MonoBehaviour {
	// ROMタイプ定義
	public enum RegionType {
		JA,
		WW,
	};

	// ROMタイプ
	public static RegionType Region { get; set; }

	//Taskキャンセル用のトークン
	private static CancellationTokenSource cancelTokenSource_;

	// Start is called before the first frame update
	void Start() {
		PlayerPrefsUtil.SetString("battle", "aiueo");


		Region = RegionType.WW;
		Debug.Log(Region.ToString());
;		//cancelTokenSource_ = new CancellationTokenSource();
		//Task1();
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			cancelTokenSource_.Cancel();
			cancelTokenSource_.Dispose();
			cancelTokenSource_ = new CancellationTokenSource();
			Debug.Log("cancel");
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Task1();
			Debug.Log("restart");
		}
	}


	void Task1() {
		Task<int> t1 = CorrespondAsync("test1", 0, true);
		Task<int> t2 = CorrespondAsync("test2", 5000, true);
		Task<int> t3 = CorrespondAsync("test3", 5000, true);
		Task<int> t4 = CorrespondAsync("test4", 5000, true);
		Debug.Log("call end");
	}


	//直列実行用のカレントTask
	private static Task currentCmd = Task.CompletedTask;
	//同時最大数制御用
	private SemaphoreSlim semaphore_ = new SemaphoreSlim(2);

	/// <summary>
	/// 非同期通信処理
	/// </summary>
	public async Task<int> CorrespondAsync(string log, int delay, bool isParallel) {
		Debug.LogWarning("count = " + semaphore_.CurrentCount);
		CancellationToken token = cancelTokenSource_.Token;
		try {
			Task<int> correspond;
			//並列実行
			if (isParallel) {
				//ロックを取得
				await semaphore_.WaitAsync(token);
				token.ThrowIfCancellationRequested();
				try {
					correspond = CorrespondAsyncImpl(log, delay, token);
					await correspond;
				} finally {
					//ロック解放
					semaphore_.Release();
				}
				//直列実行
			} else {
				correspond = currentCmd.ContinueWith(a => CorrespondAsyncImpl(log, delay, token), token).Unwrap();
				currentCmd = correspond;
				await correspond;
			}

			Debug.Log(log + ":" + correspond.Result);
			return correspond.Result;
		} catch (OperationCanceledException e) {
			Debug.LogWarning($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
			return 0;
		}
	}
	/// <summary>
	/// 非同期通信処理
	/// </summary>
	public async Task<int> CorrespondAsyncImpl(string log, int delay, CancellationToken token) {
		await Task.Delay(delay / 2);
		token.ThrowIfCancellationRequested();
		Debug.Log(log + ":half");
		await Task.Delay(delay / 2);

		return (int)(System.DateTime.Now.Ticks / 10000000);
	}
}
