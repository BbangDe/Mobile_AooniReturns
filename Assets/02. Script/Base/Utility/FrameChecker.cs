using UnityEngine;
using System.Collections;

public class FrameChecker : MonoBehaviour
{
    private float deltaTime = 0.0f;

    private GUIStyle style;
    private Rect rect;
    private float msec;
    private float fps;
    private float worstFps = 100f;
    private string text;

    private void Awake()
    {
        int w = Screen.width, h = Screen.height;

        rect = new Rect(0, 0, w, h * 4 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 1 / 100;
        style.normal.textColor = Color.cyan;

        StartCoroutine("worstReset");
    }

    private IEnumerator ResetWorst() // Reset the lowest frame rate every 15 seconds with a coroutine
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            worstFps = 100f;
        }
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI() // Display GUI as source
    {
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  // Frames per second

        if (fps < worstFps)  // If a new lowest fps is found, change worstFps
            worstFps = fps;
        text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1");
        GUI.Label(rect, text, style);
    }
}
