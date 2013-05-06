using UnityEngine;

public class MoveTask : Task {
	
	public MoveTaskScript TaskScript { get; protected set; }
	public Transform targetTransform { get; protected set; }
	public Vector3? targetPoint { get; protected set; }
	
	public MoveTask(MoveTaskScript taskScript, Transform target) : base() {
		TaskScript = taskScript;
		targetTransform = target;
	}
	
	public MoveTask(MoveTaskScript taskScript, Vector3? target) : base() {
		TaskScript = taskScript;
		targetPoint = target;
	}

	public override void Start() {
		base.Start();
		
		if(targetTransform != null) {
			TaskScript.StartTask(targetTransform);
		} else if(targetPoint != null) {
			TaskScript.StartTask(targetPoint);
		}
	}
	
	public override bool IsRunning() {
		return TaskScript.IsTaskRunning();
	}
	
	public override void Abort() {
		base.Abort();
		
		TaskScript.StopTask();
	}
	
	public override void Pause() {
		base.Pause();
		
		TaskScript.StopTask();
	}
}
