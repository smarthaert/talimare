using UnityEngine;
using System.Collections;

public abstract class ActionScript : MonoBehaviour {
	public abstract void StartAction(object target);
	public abstract bool IsActing();
	public abstract void StopAction();
}

