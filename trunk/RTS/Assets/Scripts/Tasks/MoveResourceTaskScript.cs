using UnityEngine;
using System.Collections.Generic;

// Handles moving of resources
[RequireComponent(typeof(Controllable))]
[RequireComponent(typeof(MoveTaskScript))]
[AddComponentMenu("Tasks/Move Resource")]
public class MoveResourceTaskScript : MonoBehaviour {
	
	// The maximum amount of one resource this unit can hold
	public int heldResourceLimit = 10;
	
	// The current job this unit is tasked to complete
	protected MoveResourceJob MoveResourceJob { get; set; }
	
	// Resource amount this unit is currently holding
	public ResourceAmount HeldResource { get; protected set; }
	
	// Depot this unit is currently fetching resources from
	protected ResourceDepot _depotFetchTarget;
	protected ResourceDepot DepotFetchTarget
	{
		get {
			return _depotFetchTarget;
		}
		set {
			_depotFetchTarget = value;
			depotRefreshTimer = MoveTaskScript.repathRate;
		}
	}
	
	// Depot this unit is currently returning resources to
	protected ResourceDepot _depotReturnTarget;
	protected ResourceDepot DepotReturnTarget
	{
		get {
			return _depotReturnTarget;
		}
		set {
			_depotReturnTarget = value;
			depotRefreshTimer = MoveTaskScript.repathRate;
		}
	}
	
	// Timer to signal when the current depot reference should be refreshed
	protected float depotRefreshTimer = 0f;
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if (DepotReturnTarget != null) {
			UpdateDepotReturn();
		} else if(MoveResourceJob != null) {
			if(MoveResourceJob.Completed || MoveResourceJob.Destination == null) {
				StopTask();
			} else {
				UpdateMoveResource();
			}
		}
	}
	
	// Update loop for the case where resources are being move from a depot to a destination
	protected void UpdateMoveResource() {
		if(HoldingRequiredResource()) {
			if(IsInRange(MoveResourceJob.Destination.gameObject)) {
				// In range, store resources in target
				MoveTaskScript.StopTask();
				int movedResourceAmount = Mathf.Min(HeldResource.amount, MoveResourceJob.Amount);
				MoveResourceJob.MoveResource(movedResourceAmount);
				if(movedResourceAmount == HeldResource.amount) {
					HeldResource = null;
				} else {
					HeldResource.amount -= movedResourceAmount;
				}
			} else {
				// Not in range, make sure we're moving toward target
				MoveTaskScript.StartTask(MoveResourceJob.Destination.transform);
			}
		} else {
			// Check for a new depot fetch target
			if(DepotFetchTarget == null || depotRefreshTimer <= 0f) {
				DepotFetchTarget = ResourceDepot.FindNearestDepotWithResource(MoveResourceJob.Resource, Controllable.Owner, transform.position);
				if(DepotFetchTarget == null) {
					Debug.Log("No "+MoveResourceJob.Resource+" is available to move as requested. Stopping task.");
					StopTask();
				}
			} else {
				depotRefreshTimer -= Time.deltaTime;
			}
			
			if(DepotFetchTarget != null) {
				// Make sure this depot still has the resource we need...
				if(DepotFetchTarget.GetResourceAmount(MoveResourceJob.Resource) > 0) {
					if(IsInRange(DepotFetchTarget.gameObject)) {
						// In range, fetch resources from DepotFetchTarget
						MoveTaskScript.StopTask();
						// Currently, a unit can fetch an unlimited amount of resource from a depot instantly
						HoldResource(MoveResourceJob.Resource, DepotFetchTarget.WithdrawResource(MoveResourceJob.Resource, Mathf.Min(MoveResourceJob.AmountRemaining, heldResourceLimit)));
					} else {
						// Not in range, make sure we're moving toward DepotFetchTarget
						MoveTaskScript.StartTask(DepotFetchTarget.transform);
					}
				} else {
					// ...and if not, ditch it
					DepotFetchTarget = null;
				}
			}
		}
	}
	
	// Update loop for the case where held resources are being returned to a depot.
	// DepotReturnTarget is assumed to be non-null
	protected void UpdateDepotReturn() {
		// Check for a new depot return target
		if(depotRefreshTimer <= 0f) {
			FindDepotReturnTarget();
		} else {
			depotRefreshTimer -= Time.deltaTime;
		}
		
		if(IsInRange(DepotReturnTarget.gameObject)) {
			// In range, deposit resources in DepotReturnTarget
			MoveTaskScript.StopTask();
			DepotReturnTarget.DepositResource(HeldResource.resource, HeldResource.amount);
			HeldResource = null;
			DepotReturnTarget = null; //don't call StopTask because we may still have a MoveResourceJob to complete
		} else {
			// Not in range, make sure we're moving toward DepotReturnTarget
			MoveTaskScript.StartTask(DepotReturnTarget.transform);
		}
	}
	
	protected void FindDepotReturnTarget() {
		DepotReturnTarget = ResourceDepot.FindNearestDepotForResource(HeldResource.resource, Controllable.Owner, transform.position);
		if(DepotReturnTarget == null) {
			Debug.Log("No depot is available to return "+HeldResource.resource+" to as requested. Stopping task.");
			StopTask();
		}
	}
	
	public void StartTask(MoveResourceJob moveResourceJob) {
		if(MoveResourceJob != moveResourceJob) {
			MoveResourceJob = moveResourceJob;
			// Return any other incorrect held resources to a depot before beginning
			if(HeldResourceSpaceAvailable(MoveResourceJob.Resource) == 0) {
				StartTask();
			}
		}
	}
	
	//TODO med: clicking on a depot should return any held resources to it
	// Starts this task as a special case where all currently-held resources will be moved to the nearest depot
	public void StartTask() {
		if(HeldResource != null) {
			FindDepotReturnTarget();
		}
	}
	
	public bool IsTaskRunning() {
		return MoveResourceJob != null || DepotReturnTarget != null;
	}
	
	public void StopTask() {
		if(MoveResourceJob != null) {
			MoveResourceJob.RemoveAssignee(Controllable);
			MoveResourceJob = null;
		}
		DepotReturnTarget = null;
		DepotFetchTarget = null;
		MoveTaskScript.StopTask();
	}
	
	// Holds the given amount of the given resource. The hold is only successful if the same or no resource is already held
	public void HoldResource(Resource resource, int amount) {
		if(HeldResource != null && HeldResource.resource != resource) {
			Debug.LogError("Trying to hold resource "+resource+" while already holding "+HeldResource.resource);
		}
		if(HeldResource == null && amount > 0) {
			HeldResource = new ResourceAmount(resource, amount);
		} else if(HeldResource.resource == resource) {
			HeldResource.amount += amount;
		}
		if(HeldResource.amount > heldResourceLimit) {
			Debug.LogError("Held resource amount is greater than limit. "+HeldResource.resource+" is "+HeldResource.amount+" over "+heldResourceLimit);
		}
	}
	
	// Returns the amount of resource that can be held. Since only one resource can be held at a time, if another resource is currently held then 0 is returned
	public int HeldResourceSpaceAvailable(Resource resource) {
		if(HeldResource != null) {
			if(HeldResource.resource != resource) {
				return 0;
			} else {
				return heldResourceLimit - HeldResource.amount;
			}
		} else {
			return heldResourceLimit;
		}
	}
		
	public bool HoldingRequiredResource() {
		return HeldResource != null && HeldResource.resource == MoveResourceJob.Resource;
	}
	
	protected bool IsInRange(GameObject gameObject) {
		float range = this.collider.bounds.size.magnitude/2 + gameObject.collider.bounds.size.magnitude/2 + 0.5f;
		return (gameObject.transform.position - this.transform.position).magnitude <= range;
	}
}
