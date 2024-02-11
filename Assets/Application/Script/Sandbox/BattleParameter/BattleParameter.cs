using UnityEngine;

[ExecuteAlways]
public class BattleParameter : MonoBehaviour
{
	public int[] in_value;
	public int[] out_value;
	public int start = 0;

	// Start is called before the first frame update
	void Start()
    {
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			out_value[1] = 0;
			out_value[2] = 0;
			out_value[3] = 0;
			out_value[4] = 0;
			out_value[0] = 0;

			int current = start;
			for(int i = 0; i < 5; i++) {
				int index = (current + 1) % 5;
				int add = current % 5;
				int minus = (current - 1 + 5) % 5;
				out_value[index] = in_value[add] + out_value[add] - in_value[minus] - out_value[minus];
				current++;
			}

			/*out_value[1] = in_value[0] + out_value[0] - in_value[4] - out_value[4];
			out_value[2] = in_value[1] + out_value[1] - in_value[0] - out_value[0];
			out_value[3] = in_value[2] + out_value[2] - in_value[1] - out_value[1];
			out_value[4] = in_value[3] + out_value[3] - in_value[2] - out_value[2];
			out_value[0] = in_value[4] + out_value[4] - in_value[3] - out_value[3];*/
		}
	}
}
