
public class MoveResourceTask : Task {
	
	public MoveResourceTaskScript TaskScript { get; protected set; }
	public MoveResourceJob Job { get; protected set; }
	
	public MoveResourceTask(MoveResourceTaskScript taskScript, MoveResourceJob job) : base() {
		TaskScript = taskScript;
		Job = job;
	}

	public override void Start() {
		base.Start();
		
		TaskScript.StartTask(Job);
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
