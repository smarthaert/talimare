using UnityEngine;

public class GatherTask : Task {
	
	public GatherTaskScript TaskScript { get; protected set; }
	public ResourceNode Target { get; protected set; }
	
	public GatherTask(GatherTaskScript taskScript, ResourceNode target) : base() {
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
