using UnityEngine;

public class CameraRotate : MonoBehaviour {
	public float rotateSpeed = 5;
	
	protected float defaultPointDistance = 10;
	
	protected Vector3 screenCenter;
	
	void Start () {
		screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0);
	}
	
	void FixedUpdate () {
		float rotateAngle = Input.GetAxis("CamRotate") * rotateSpeed;
		if(rotateAngle != 0) {
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			RaycastHit hit;
			Vector3 targetPoint;
			if(Physics.Raycast(ray, out hit)) {
				targetPoint = hit.point;
			} else {
				targetPoint = ray.GetPoint(transform.position.y);
			}
			Debug.Log(targetPoint);
			
			transform.RotateAround(targetPoint, Vector3.up, rotateAngle * Time.deltaTime);
		}
	}
}