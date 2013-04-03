using UnityEngine;

// Handles unit gathering
[RequireComponent(typeof(MoveTaskScript))]
public class GatherTaskScript : TaskScript {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	// The maximum amount of one resource this unit can gather before returning to a depot
	public int gatherLimit;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	// Resource amount this unit is currently holding
	protected ResourceAmount HeldResource { get; set; }
	
	// The resource node this unit is currently gathering from
	protected ResourceNode GatherTarget { get; set; }
	protected float GatherTimer { get; set; }
	// The depot this unit is currently returning resources to
	protected ResourceDepot DepotTarget { get; set; }
	
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected override void Awake() {
		base.Awake();
		
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if(DepotTarget != null) {
			UpdateDepotReturn();
		} else if(GatherTarget != null) {
			UpdateGather();
		}
	}
	
	protected void UpdateDepotReturn() {
		if(IsInRange(DepotTarget.gameObject)) {
			// In range, deposit resources
			MoveTaskScript.StopTask();
			DepotTarget.DepositResource(HeldResource.resource, HeldResource.amount);
			DepotTarget = null;
			HeldResource = null;
		} else {
			// Not in range, make sure we're moving toward depot
			MoveTaskScript.StartTask(DepotTarget.transform);
		}
	}
	
	protected void UpdateGather() {
		if(GatherTimer > 0) {
			// Currently gathering
			GatherTimer -= Time.deltaTime;
			if(GatherTimer <= 0) {
				// Timer's up, trigger gather from node if still in range
				if(IsInRange(GatherTarget.gameObject)) {
					int gatheredAmount = GatherTarget.GatherFrom(gatherAmount);
					if(HeldResource == null) {
						HeldResource = new ResourceAmount();
						HeldResource.resource = GatherTarget.resource;
						HeldResource.amount = 0;
					}
					HeldResource.amount += gatheredAmount;
					if(HeldResource.amount >= gatherLimit) {
						DepotTarget = ResourceDepot.FindNearestDepotForResource(transform.position, Controllable.owner, HeldResource.resource);
					} else {
						GatherTimer = gatherTime;
					}
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInRange(GatherTarget.gameObject)) {
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
			if(HeldResource != null && HeldResource.resource != GatherTarget.resource) {
				HeldResource = null;
			}
		}
	}
	
	public override bool IsTaskRunning() {
		return GatherTarget != null;
	}
	
	public override void StopTask() {
		GatherTarget = null;
		DepotTarget = null;
	}
	
	protected bool IsInRange(GameObject gameObject) {
		float range = this.collider.bounds.size.magnitude/2 + gameObject.collider.bounds.size.magnitude/2 + 0.5f;
		return (gameObject.transform.position - this.transform.position).magnitude <= range;
	}
}
