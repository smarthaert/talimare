using UnityEngine;

// Handles unit gathering
[RequireComponent(typeof(MoveTaskScript))]
public class GatherTaskScript : TaskScript {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	// The resource node this unit is currently gathering
	protected ResourceNode GatherTarget { get; set; }
	protected float GatherTimer { get; set; }
	
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		MoveTaskScript = GetComponent<MoveTaskScript>();
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
					GatherTarget.GatherFrom(gatherAmount);
					//TODO return resource to depot
					//Player.PlayerStatus.GainResource(GatherTarget.resource, gatherAmount);
					GatherTimer = gatherTime;
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInGatherRange()) {
				// In range, start gathering
				MoveTaskScript.StopTask();
				GatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				MoveTaskScript.StartTask(GatherTarget.transform);
			}
		}
	}
	
	public override void StartTask(object target) {
		if(GatherTarget != (ResourceNode)target) {
			GatherTarget = (ResourceNode)target;
			GatherTimer = 0;
		}
	}
	
	public override bool IsTaskRunning() {
		return GatherTarget != null;
	}
	
	public override void StopTask() {
		GatherTarget = null;
	}
	
	protected bool IsInGatherRange() {
		float gatherRange = this.collider.bounds.size.magnitude/2 + GatherTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (GatherTarget.transform.position - this.transform.position).magnitude <= gatherRange;
	}
}
