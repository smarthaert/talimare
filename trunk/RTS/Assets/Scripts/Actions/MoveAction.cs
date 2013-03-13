using UnityEngine;

public class MoveAction : Action {
	
	public Vector3 Destination { get; set; }
	public AIPathfinder Pathfinder { get; set; }
	
	public MoveAction(Controllable actor, Vector3 destination) : base(actor) {
		Destination = destination;
		Pathfinder = Actor.GetComponent<AIPathfinder>();
	}
	
	public override void Start() {
		base.Start();
		
		Pathfinder.Move(Destination);
	}

	public override void Update() {
		base.Update();
		
		if(!Pathfinder.IsMoving()) {
			Finish();
		}
	}

	public override void Finish() {
		base.Finish();
	}
	
	public override void Abort() {
		base.Abort();
		Pathfinder.StopMoving();
	}
}