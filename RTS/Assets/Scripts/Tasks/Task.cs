
// A Task is a TaskScript linked with a job or target, and can be queued inside Controllables
public abstract class Task {
	
	public bool Started { get; protected set; }
	
	public Task() {
		Started = false;
	}
	
	public virtual void Start() {
		Started = true;
	}
	public abstract bool IsRunning();
	public virtual void Abort() {}
	public virtual void Pause() {
		Started = false;
	}
}
