// This class is used to link ActionScripts with targets, and to queue those links inside Controllables
public class Action {
	
	public ActionScript ActionScript { get; set; }
	public object Target { get; set; }
	public bool IsStarted { get; set; }
	
	public Action(ActionScript actionScript, object target) {
		ActionScript = actionScript;
		Target = target;
		IsStarted = false;
	}

	public void Start() {
		ActionScript.StartAction(Target);
		IsStarted = true;
	}
	
	public bool IsRunning() {
		return ActionScript.IsActing();
	}
	
	public void Abort() {
		ActionScript.StopAction();
	}
}
