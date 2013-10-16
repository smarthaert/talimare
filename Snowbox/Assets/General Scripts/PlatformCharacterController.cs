using UnityEngine;
using System.Collections;

public class PlatformCharacterController : MonoBehaviour
{
	
	private CharacterMotor motor;
	
	public float walkMultiplier = 0.5f;
	public bool defaultIsWalk = false;

	public string findAimByTag = "";
	public Transform aimTarget;
	public float maxHorizontalAimAngle = 50f;
	
	// Use this for initialization
	void Start ()
	{
		motor = GetComponent<CharacterMotor>();
		if (motor==null) Debug.Log("Motor is null!!");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!Screen.lockCursor)
		{
			return;
		}

		if (aimTarget == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag(findAimByTag);
			if (go)
			{
				aimTarget = go.transform;
			}
			else
			{
				return;
			}
		}

		// Get input vector from keyboard or analog stick and make it length 1 at most
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		if (directionVector.magnitude>1) directionVector = directionVector.normalized;
		directionVector = directionVector.normalized * Mathf.Pow(directionVector.magnitude, 2);
		
		// Rotate input vector into camera space so up is camera's up and right is camera's right
		directionVector = Camera.main.transform.rotation * directionVector;
		
		// Rotate input vector to be perpendicular to character's up vector
		Quaternion camToCharacterSpace = Quaternion.FromToRotation(Camera.main.transform.forward*-1, transform.up);
		directionVector = (camToCharacterSpace * directionVector);
		
		// Make input vector relative to Character's own orientation
		directionVector = Quaternion.Inverse(transform.rotation) * directionVector;
		
		if (walkMultiplier!=1)
		{
			if ( (Input.GetKey("left shift") || Input.GetKey("right shift")) != defaultIsWalk )
			{
				directionVector *= walkMultiplier;
			}
		}
		
		// Apply direction
		motor.desiredMovementDirection = directionVector;

		// Find vector towards aimTarget
		Vector3 aimTargetVector = aimTarget.position - transform.position;
		aimTargetVector.y = 0;
		// Always apply direction right away when moving.
		// When standing still, apply direction when it's more than given treshold,
		// so character at first aims horizontaly and only when the aim target is
		// out of reach it will turn whole body.
		if (motor.desiredMovementDirection.sqrMagnitude > 0.1f ||
			Vector3.Angle(transform.forward, aimTargetVector) > maxHorizontalAimAngle)
		{
			motor.desiredFacingDirection = aimTargetVector;
		}
	}
}
