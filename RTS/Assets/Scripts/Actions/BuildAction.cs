using UnityEngine;

public class BuildAction : Action {
	
	public BuildProgressControl Target { get; set; }
	public AIBuilder Builder { get; set; }
	
	public BuildAction(Controllable actor, BuildProgressControl target) : base(actor) {
		Target = target;
		Builder = Actor.GetComponent<AIBuilder>();
	}
	
	public override void Start() {
		base.Start();
		
		Builder.Build(Target);
	}

	public override void Update() {
		base.Update();
		
		if(!Builder.IsBuilding()) {
			Finish();
		}
	}

	public override void Finish() {
		base.Finish();
	}
	
	public override void Abort() {
		base.Abort();
		Builder.StopBuilding();
	}
}