using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int fps = Mathf.RoundToInt(1.0f / deltaTime);

        GUIStyle style = new();
        style.fontSize = 40;
        style.normal.textColor = Color.red;

        float width = 200f;
        float height = 40f;
        float x = Screen.width - width - 20f; 
        float y = Screen.height - height - 20f;

        GUI.Label(new Rect(x, y, width, height), "FPS : " + fps, style);
    }
}