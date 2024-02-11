using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshRefresh : MonoBehaviour
{
	float t = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		t += Time.deltaTime;
		if(t > 5f) {
			GetComponent<NavMeshSurface>().BuildNavMesh();
			t = 0f;
		}


	}
}
