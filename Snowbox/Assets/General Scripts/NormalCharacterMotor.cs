using UnityEngine;
using System.Collections;

[RequireComponent(typeof(uLinkStrictPlatformer))]
public class NormalCharacterMotor : CharacterMotor
{	
	public float maxRotationSpeed = 270;
	
	private uLinkStrictPlatformer platformer;
	
	void Awake()
	{
		 platformer = GetComponent<uLinkStrictPlatformer>();
	}
	
	private void UpdateFacingDirection() {
		// Calculate which way character should be facing
		float facingWeight = desiredFacingDirection.magnitude;
		Vector3 combinedFacingDirection = (
			transform.rotation * desiredMovementDirection * (1-facingWeight)
			+ desiredFacingDirection * facingWeight
		);
		combinedFacingDirection = Util.ProjectOntoPlane(combinedFacingDirection, transform.up);
		combinedFacingDirection = alignCorrection * combinedFacingDirection;
		
		if (combinedFacingDirection.sqrMagnitude > 0.01f) {
			Vector3 newForward = Util.ConstantSlerp(
				transform.forward,
				combinedFacingDirection,
				maxRotationSpeed*Time.deltaTime
			);
			newForward = Util.ProjectOntoPlane(newForward, transform.up);
			//Debug.DrawLine(transform.position, transform.position+newForward, Color.yellow);
			Quaternion q = new Quaternion();
			q.SetLookRotation(newForward, transform.up);
			transform.rotation = q;
		}
	}
	
	private void UpdateVelocity()
	{
		platformer.SetInput(desiredVelocity.x, desiredVelocity.z, Input.GetButtonDown("Jump"));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		
		UpdateFacingDirection();
		
		if (!ChatClient.isEnabled)
		{
			UpdateVelocity();
		}
	}
}
