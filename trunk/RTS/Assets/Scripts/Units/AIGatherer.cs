using UnityEngine;

// Handles unit gathering
[RequireComponent(typeof(AIPathfinder))]
public class AIGatherer : MonoBehaviour {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	// The resource node this unit is currently gathering
	protected ResourceNode gatherTarget;
	protected float gatherTimer = 0;
	
	protected Player player;
	protected AIPathfinder pathfinder;
	
	protected void Start() {
		player = GetComponent<Controllable>().owner;
		pathfinder = GetComponent<AIPathfinder>();
	}
	
	protected void Update () {
		if(gatherTarget != null) {
			UpdateGather();
		}
	}
	
	protected void UpdateGather() {
		if(gatherTimer > 0) {
			// Currently gathering
			gatherTimer -= Time.deltaTime;
			if(gatherTimer <= 0) {
				// Timer's up, trigger gather from node if still in range
				if(IsInGatherRange()) {
					gatherTarget.Gather(gatherAmount);
					player.PlayerStatus.GainResource(gatherTarget.resource, gatherAmount);
					gatherTimer = gatherTime;
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInGatherRange()) {
				// In range, start gathering
				pathfinder.StopMoving();
				gatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				pathfinder.Move(gatherTarget.transform);
			}
		}
	}
	
	// Sets this unit to gather from the given resource node
	public void Gather(ResourceNode node) {
		if(node != gatherTarget) {
			gatherTimer = 0;
			gatherTarget = node;
		}
	}
	
	public void StopGathering() {
		gatherTarget = null;
	}
	
	public bool IsGathering() {
		return gatherTarget = null;
	}
	
	protected bool IsInGatherRange() {
		float gatherRange = this.collider.bounds.size.magnitude/2 + gatherTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (gatherTarget.transform.position - this.transform.position).magnitude <= gatherRange;
	}
}
