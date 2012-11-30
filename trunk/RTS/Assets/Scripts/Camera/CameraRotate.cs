using UnityEngine;

public class CameraRotate : MonoBehaviour {
	public float rotateSpeed = 250;
	
	protected float defaultPointDistance = 10;
	
	protected Vector3 screenCenter;
	protected int layerMask;
	
	void Start () {
		screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0);
		layerMask = ~(1 << LayerMask.NameToLayer("FogOfWar"));
	}
	
	void LateUpdate () {
		float rotateAngle = Input.GetAxis("CamRotate") * rotateSpeed;
		if(rotateAngle != 0) {
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			RaycastHit hit;
			Vector3 targetPoint;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
				targetPoint = hit.point;
			} else {
				targetPoint = ray.GetPoint(transform.position.y);
			}
			
			transform.RotateAround(targetPoint, Vector3.up, rotateAngle * Time.deltaTime);
		}
	}
}