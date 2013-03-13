// The base class for all actions which can be completed by in-game entities
public abstract class Action {
	
	public bool IsStarted { get; set; }
	public Controllable Actor { get; set; }
	
	public Action(Controllable actor) {
		IsStarted = false;
		Actor = actor;
	}

	public virtual void Start() {
		IsStarted = true;
	}
	
	public virtual void Update() {}
	
	public virtual void Finish() {
		IsStarted = false;
	}
	
	public virtual void Abort() {
		IsStarted = false;
	}
}
