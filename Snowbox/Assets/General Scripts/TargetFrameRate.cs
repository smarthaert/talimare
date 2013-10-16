using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
	public int frameRate = 60;

	void Awake ()
	{
		Application.targetFrameRate = frameRate;
	}
}
