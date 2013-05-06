
public class BuildTask : Task {
	
	public BuildTaskScript TaskScript { get; protected set; }
	public BuildJob Job { get; protected set; }
	
	public BuildTask(BuildTaskScript taskScript, BuildJob job) : base() {
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
