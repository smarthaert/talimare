using UnityEngine;
using System.Collections;

public class EasterEgg : MonoBehaviour
{
	public float sqrMinDist = 10;
	public float sqrMaxDist = 100;
	
	private Vector3 pos;
	private Material mat;
	
	void Awake()
	{
		pos = transform.position;
		mat = renderer.material;
	}
	
	void Update ()
	{
		Camera cam = Camera.main;
		if (cam == null) return;
		
		float sqrDist = (pos - cam.transform.position).sqrMagnitude;
		
		sqrDist = Mathf.Clamp(sqrDist, sqrMinDist, sqrMaxDist);
		
		float value = Mathf.SmoothStep(0, 1, 1 - (sqrDist - sqrMinDist) / (sqrMaxDist - sqrMinDist));
		
		mat.color = new Color(1, 1, 1, value);
	}
}
