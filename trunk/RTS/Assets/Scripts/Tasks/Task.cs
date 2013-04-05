// A Task is a TaskScript linked with a target, and can be queued inside Controllables
public class Task {
	
	public TaskScript TaskScript { get; set; }
	public object Target { get; set; }
	public bool IsStarted { get; set; }
	
	public Task(TaskScript taskScript, object target) {
		TaskScript = taskScript;
		Target = target;
		IsStarted = false;
	}

	public void Start() {
		TaskScript.StartTask(Target);
		IsStarted = true;
	}
	
	public bool IsRunning() {
		return TaskScript.IsTaskRunning();
	}
	
	public void Abort() {
		TaskScript.StopTask();
	}
	
	public void Pause() {
		TaskScript.StopTask();
		IsStarted = false;
	}
}
