using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/Camera Move")]
public class CameraMove : MonoBehaviour {
	
	public float moveSpeed = 30;

	protected Vector3 moveDirection = Vector3.zero;
	
	void LateUpdate () {
		if(Input.GetAxis("CamHorizontal") != 0 || Input.GetAxis("CamVertical") != 0) {
			moveDirection = new Vector3(Input.GetAxis("CamHorizontal"), 0, Input.GetAxis("CamVertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= moveSpeed;
			transform.position += moveDirection * Time.deltaTime;
		}
	}
}
