using UnityEngine;

public class GatherAction : Action {
	
	public ResourceNode Target { get; set; }
	public AIGatherer Gatherer { get; set; }
	
	public GatherAction(Controllable actor, ResourceNode target) : base(actor) {
		Target = target;
		Gatherer = Actor.GetComponent<AIGatherer>();
	}
	
	public override void Start() {
		base.Start();
		
		Gatherer.Gather(Target);
	}

	public override void Update() {
		base.Update();
		
		if(!Gatherer.IsGathering()) {
			Finish();
		}
	}

	public override void Finish() {
		base.Finish();
	}
	
	public override void Abort() {
		base.Abort();
		Gatherer.StopGathering();
	}
}