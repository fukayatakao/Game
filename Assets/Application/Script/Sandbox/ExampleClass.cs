using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public Rect windowRect = new Rect(20, 20, 220, 150);

    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "My Window");
    }

    // Make the contents of the window
    void DoMyWindow(int windowID)
    {
        GUI.Button(new Rect(10, 20, 100, 20), "Can't drag me");
        // Insert a huge dragging area at the end.
        // This gets clipped to the window (like all other controls) so you can never
        //  drag the window from outside it.
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
