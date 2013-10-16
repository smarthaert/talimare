using UnityEngine;
using System.Collections;

public class HidePlaneBackside : MonoBehaviour
{
	public Vector3 planeNormal = Vector3.up;
	
	// Update is called once per frame
	void Update ()
	{
		Camera cam = Camera.main;
		if (cam == null) return;
		
		Vector3 dir = transform.position - cam.transform.position;
		Vector3 normal = transform.rotation * planeNormal;
		float cosAngle = Vector3.Dot(normal, dir);
		
		renderer.enabled = (cosAngle < 0); 
	}
}
