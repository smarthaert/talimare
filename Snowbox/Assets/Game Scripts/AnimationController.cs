using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
	public Transform character = null;

	private Vector3 prevPos = Vector3.zero;

	void Start()
	{
		// Add a duplicate shoot animation which we set up to only animate the upper body
		// We use this animation when the character is running.
		// By using mixing for this we dont need to make a seperate running-shoot animation
		animation.AddClip(animation["Throw"].clip, "ThrowUpperBody");
		animation["ThrowUpperBody"].AddMixingTransform(transform.Find("Armature.001/Mover/SpineRoot"));

		// Set all animations to loop
		animation.wrapMode = WrapMode.Loop;

		// Except our action animations, Dont loop those
		animation["JumpWave"].wrapMode = WrapMode.Clamp;
		animation["Throw"].wrapMode = WrapMode.Clamp;
		animation["ThrowUpperBody"].wrapMode = WrapMode.Clamp;

		// Put idle and run in a lower layer. They will only animate if our action animations are not playing
		animation["StandCraze"].layer = -1;
		animation["Run"].layer = -1;
		animation["StepLeft"].layer = -1;
		animation["StepRight"].layer = -1;

		animation.Stop();
		
		prevPos = character.position;
	}
	
	void Update()
	{
		Vector3 velocity = character.position - prevPos;
		prevPos = character.position;

		float vertical = Vector3.Dot(character.forward, velocity);
		float horizontal = Vector3.Dot(character.right, velocity);

		if (Mathf.Abs(vertical) > 0.1f)
		{
			animation.CrossFade("Run");
			// Play animation backwards when running backwards
			animation["Run"].speed = Mathf.Sign(vertical);
		}
		else if (horizontal < -0.1f)
		{
			animation.CrossFade("StepLeft");
		}
		else if (horizontal > 0.1f)
		{
			animation.CrossFade("StepRight");
		}
		else
		{
			animation.CrossFade("StandCraze");
		}
	}

	public void Throw()
	{
		// Play it only on the upper body
		animation.CrossFadeQueued("ThrowUpperBody", 0.1f, QueueMode.PlayNow);
	}
}
