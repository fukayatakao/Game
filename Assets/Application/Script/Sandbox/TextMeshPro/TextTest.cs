using UnityEngine;

public class TextTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		var pro = gameObject.GetComponent<TMPro.TextMeshPro>();
		pro.text = "あいうえお";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
