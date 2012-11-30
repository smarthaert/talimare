using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public float moveSpeed = 30f;
	
	public float zoomSpeed = 5f;
	public float zoomUpperLimit = 90f;
	public float zoomLowerLimit = 5f;

	private Vector3 moveDirection = Vector3.zero;

	void Start () {}
	
	void FixedUpdate () {
		// Move
		moveDirection = new Vector3(Input.GetAxis("CamHorizontal"), 0, Input.GetAxis("CamVertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= moveSpeed;
		transform.position += moveDirection * Time.deltaTime;
		
		// Zoom
		if (((Input.GetAxis("CamZoom") > 0) && (transform.localPosition.z < -zoomLowerLimit)) ||
				((Input.GetAxis("CamZoom") < 0) && (transform.localPosition.z > -zoomUpperLimit))) {
			moveDirection = new Vector3(0, 0, Input.GetAxis("CamZoom"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= zoomSpeed;
			transform.position += moveDirection * Time.deltaTime;
		}
	}
}
