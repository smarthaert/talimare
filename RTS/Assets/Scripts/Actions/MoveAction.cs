using UnityEngine;

public class MoveAction : Action {
	
	public Vector3 Destination { get; set; }
	public AIPathfinder Pathfinder { get; set; }
	
	public MoveAction(Vector3 destination, AIPathfinder pathfinder) {
		Destination = destination;
		Pathfinder = pathfinder;
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