using UnityEngine;
using Project.Lib;
using Project.Game;

#if DEVELOP_BUILD

public class TestSceneMain : MonoBehaviour
{
    PrimitiveFan primitive;

    // Start is called before the first frame update
    void Start()
    {
        primitive = new PrimitiveFan();
        //primitive = new PrimitiveSphere();
		primitive.Create("test");


        primitive.SetFan(5f, 360f * Mathf.Deg2Rad);
        primitive.SetColor(Color.blue);
		DebugWindowManager.Create(typeof(DebugRootMenu));
	}

	// Update is called once per frame
	void Update()
    {

    }

	Vector2 scrollPosition = Vector2.zero;
	void OnGUI() {
		/*GUI.skin.verticalScrollbarThumb.fontSize = 30;
		GUIStyle style = GUI.skin.verticalScrollbar;
		style.fixedWidth = 30f;*/

		/*scrollPosition = GUILayout.BeginScrollView( scrollPosition, GUILayout.Width( 500) );
		GUILayout.Button("test");
		GUILayout.Button("test");
		GUILayout.Button("test");
		/*GUILayout.Button("test");
		GUILayout.Button("test");
		GUILayout.Button("test");
		GUILayout.Button("test");*/

		//FitGUI.SelectionGrid(new Rect(50f, 50f, 500f, 164f),0, new string[] { "test1", "test2", "test3", "test4" },1);
		//GUI.DragWindow(new Rect(50f, 50f, 500f, 164f));
		//FitGUI.Box(new Rect(50f, 50f, 500f, 164f), "");
		/*float y = 50f;
		float size = 64f;
		for (int i = 0; i < 10; i++) {
			FitGUI.Button(new Rect(50f, y, 500f, size), "test1");
			y = y + size;
		}*/

		//GUILayout.EndScrollView();
	}
}
#endif
