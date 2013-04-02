using UnityEngine;
using System.Collections;

public abstract class TaskScript : MonoBehaviour {
	
	protected Controllable Controllable { get; set; }
	
	protected virtual void Awake() {
		Controllable = GetComponent<Controllable>();
	}
	
	public abstract void StartTask(object target);
	public abstract bool IsTaskRunning();
	public abstract void StopTask();
}

