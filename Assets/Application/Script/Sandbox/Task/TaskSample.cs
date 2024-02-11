using UnityEngine;
using System.Threading.Tasks;

public class TaskSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Task t = currentOperationTask.ContinueWith(a => Task1()).Unwrap();
		currentOperationTask = t;
		Task t2 = currentOperationTask.ContinueWith(a => Task2()).Unwrap();
		currentOperationTask = t2;
		Test(currentOperationTask);
		Task t3 = currentOperationTask.ContinueWith(a => Task3()).Unwrap();

	}

	// Update is called once per frame
	void Update()
    {

    }

	private Task currentOperationTask = Task.CompletedTask;

	public async Task Task1() {
		long t = System.DateTime.Now.Ticks;
		Debug.Log("task1 start:" + t / 10000f);

		await Task.Delay(0);
		Debug.Log("task1 finish:" + (System.DateTime.Now.Ticks - t) / 10000f);
	}

	public async Task Task2() {
		long t = System.DateTime.Now.Ticks;
		Debug.Log("task2 start:" + t / 10000f);
		await Task.Delay(3000);
		Debug.Log("task2 finish:" + (System.DateTime.Now.Ticks - t) / 10000f);
		Debug.Log("task2 tick:" + System.DateTime.Now.Ticks);
	}

	public async Task Task3() {
		long t = System.DateTime.Now.Ticks;
		Debug.Log("task3 start:" + t / 10000f);
		await Task.Delay(2000);
		Debug.Log("task3 finish:" + (System.DateTime.Now.Ticks - t) / 10000f);
	}


	public async void Test(Task t) {
		await t;
		Debug.Log("test call:" + System.DateTime.Now.Ticks);
	}
}
