using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
    private void Update()
    {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate = Time.time + 1.0f / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("FPS:" + fps);
    }

    private int frameCount = 0;
    private float nextUpdate = 0.0f;
    private float fps = 0.0f;
    private float updateRate = 4.0f;
}
