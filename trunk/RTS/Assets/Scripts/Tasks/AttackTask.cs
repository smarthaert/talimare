using UnityEngine;

public class AttackTask : Task {
	
	public AttackTaskScript TaskScript { get; protected set; }
	public GameObject Target { get; protected set; }
	
	public AttackTask(AttackTaskScript taskScript, GameObject target) : base() {
		TaskScript = taskScript;
		Target = target;
	}

	public override void Start() {
		base.Start();
		
		TaskScript.StartTask(Target);
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
