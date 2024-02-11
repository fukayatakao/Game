using UnityEngine;

public class FixRandomSandbox : MonoBehaviour
{
	const int MAX = 10;
    // Start is called before the first frame update
    void Start()
    {
		Random.InitState(1111);
		string log = "";
		float[] result = new float[MAX];
		for(int i = 0; i < MAX; i++) {
			result[i] = Random.Range(0f, (float)i);
			log += i + "回目:" + result[i] + "\n";
		}

		Debug.Log(log);
	}

	// Update is called once per frame
	void Update()
    {

    }
}
