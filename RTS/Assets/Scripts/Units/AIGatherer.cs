using UnityEngine;

// Handles unit gathering
[RequireComponent(typeof(AIPathfinder))]
public class AIGatherer : ActionScript {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	// The resource node this unit is currently gathering
	protected ResourceNode GatherTarget { get; set; }
	protected float GatherTimer { get; set; }
	
	protected Player Player { get; set; }
	protected AIPathfinder Pathfinder { get; set; }
	
	protected void Awake() {
		Pathfinder = GetComponent<AIPathfinder>();
	}
	
	protected void Start() {
		Player = GetComponent<Controllable>().owner;
	}
	
	protected void Update () {
		if(GatherTarget != null) {
			UpdateGather();
		}
	}
	
	protected void UpdateGather() {
		if(GatherTimer > 0) {
			// Currently gathering
			GatherTimer -= Time.deltaTime;
			if(GatherTimer <= 0) {
				// Timer's up, trigger gather from node if still in range
				if(IsInGatherRange()) {
					GatherTarget.Gather(gatherAmount);
					Player.PlayerStatus.GainResource(GatherTarget.resource, gatherAmount);
					GatherTimer = gatherTime;
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInGatherRange()) {
				// In range, start gathering
				Pathfinder.StopMoving();
				GatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				Pathfinder.Move(GatherTarget.transform);
			}
		}
	}
	
	public override void StartAction(object target) {
		if(GatherTarget != (ResourceNode)target) {
			GatherTarget = (ResourceNode)target;
			GatherTimer = 0;
		}
	}
	
	public override bool IsActing() {
		return GatherTarget != null;
	}
	
	public override void StopAction() {
		GatherTarget = null;
	}
	
	protected bool IsInGatherRange() {
		float gatherRange = this.collider.bounds.size.magnitude/2 + GatherTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (GatherTarget.transform.position - this.transform.position).magnitude <= gatherRange;
	}
}
