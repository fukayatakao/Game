using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoopSandbox : MonoBehaviour
{

	/// <summary>
	/// forとforeachの速度比較
	/// </summary>
	public static void Check() {
		// ループ回数=10万件
		int loopCnt = 100000;

		var list = createTestData(loopCnt);

		// キャッシュが効いて結果が不公平にならないように
		// 同じ処理を計測せず事前に1000回走らせておく
		for (int pp = 0; pp < 1000; pp++) {
			{
				var sw1 = System.Diagnostics.Stopwatch.StartNew();

				int sum1 = 0;
				for (int i = 0; i < list.Count; i++) {
					sum1 += list[i];
				}
				sw1.Stop();

				var sw2 = System.Diagnostics.Stopwatch.StartNew();
				int sum2 = 0;
				foreach (var p in list) {
					sum2 += p;
				}
				sw2.Stop();
			}
		}

		{
			List<double> a1 = new List<double>();
			List<double> a2 = new List<double>();
			for (int pp = 0; pp < 1000; pp++) {
				var sw1 = System.Diagnostics.Stopwatch.StartNew();

				int sum1 = 0;
				for (int i = 0, max = list.Count; i < max; i++) {
					sum1 += list[i];
				}

				sw1.Stop();
				a1.Add(sw1.Elapsed.TotalMilliseconds);

				var sw2 = System.Diagnostics.Stopwatch.StartNew();
				int sum2 = 0;
				foreach (var p in list) {
					sum2 += p;
				}
				sw2.Stop();
				a2.Add(sw2.Elapsed.TotalMilliseconds);
			}

			Debug.Log($"for, {a1.Average()}msec");
			Debug.Log($"foreach, {a2.Average()}msec");
			// > for, 0.16245250000000092msec
			// > foreach, 0.24426649999999864msec
		}
	}

	private static List<int> createTestData(int count) {
		IEnumerable<int> f() {
			for (int i = 0; i < count; i++) {
				yield return i;
			}
		}
		return f().ToList();
	}
	// Start is called before the first frame update
	void Start()
    {
		Check();

	}

    // Update is called once per frame
    void Update()
    {

    }
}
