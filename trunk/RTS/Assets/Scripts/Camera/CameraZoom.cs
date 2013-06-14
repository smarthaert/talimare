using UnityEngine;

[AddComponentMenu("Camera/Camera Zoom")]
public class CameraZoom : MonoBehaviour {
	
	public float zoomSpeed = 10;
	public float zoomLowerLimit = 20;
	public float zoomUpperLimit = 40;

	protected Vector3 moveDirection = Vector3.zero;
	
	void LateUpdate () {
		if (((Input.GetAxis("CamZoom") < 0) && (transform.position.y <= zoomUpperLimit)) ||
				((Input.GetAxis("CamZoom") > 0) && (transform.position.y >= zoomLowerLimit))) {
			moveDirection = new Vector3(0, 0, Input.GetAxis("CamZoom"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= zoomSpeed;
			transform.position += moveDirection * Time.deltaTime;
		}
	}
}
