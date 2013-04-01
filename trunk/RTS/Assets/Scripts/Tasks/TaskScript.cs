using UnityEngine;
using System.Collections;

public abstract class TaskScript : MonoBehaviour {
	public abstract void StartTask(object target);
	public abstract bool IsTaskRunning();
	public abstract void StopTask();
}

