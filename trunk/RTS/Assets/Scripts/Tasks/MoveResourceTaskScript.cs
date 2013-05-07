using UnityEngine;
using System.Collections.Generic;

// Handles moving of resources
[RequireComponent(typeof(MoveTaskScript))]
public class MoveResourceTaskScript : MonoBehaviour {
	
	// The maximum amount of one resource this unit can hold
	public int heldResourceLimit = 10;
	
	// The current job this unit is tasked to complete
	protected MoveResourceJob MoveResourceJob { get; set; }
	
	// Resource amount this unit is currently holding
	public ResourceAmount HeldResource { get; protected set; }
	// Depot this unit is currently fetching resources from
	protected ResourceDepot DepotFetchTarget { get; set; }
	// Depot this unit is currently returning resources to
	protected ResourceDepot DepotReturnTarget { get; set; }
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if(MoveResourceJob != null) {
			if(MoveResourceJob.Completed) {
				StopTask();
			} else {
				UpdateMoveResource();
			}
		} else if (DepotReturnTarget != null) {
			UpdateDepotReturn();
		}
	}
	
	protected void UpdateMoveResource() {
		if(HoldingRequiredResourceAmount()) {
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
			//TODO med: check for updated DepotFetchTarget every x seconds (equal to seeker update rate)
			if(DepotFetchTarget == null) {
				DepotFetchTarget = ResourceDepot.FindNearestDepotWithResource(transform.position, Controllable.owner, MoveResourceJob.Resource);
			}
			if(IsInRange(DepotFetchTarget.gameObject)) {
				// In range, fetch resources from DepotFetchTarget
				MoveTaskScript.StopTask();
				//TODO !!!! Fetch required resource from depot
				int fetchedResourceAmount = Mathf.Min(DepotFetchTarget.GetResourceAmount(MoveResourceJob.Resource), MoveResourceJob.AmountRemaining);
				DepotFetchTarget.WithdrawResource(MoveResourceJob.Resource, fetchedResourceAmount);
			} else {
				// Not in range, make sure we're moving toward DepotFetchTarget
				MoveTaskScript.StartTask(DepotFetchTarget.transform);
			}
		}
	}
	
	protected void UpdateDepotReturn() {
		//TODO med: check for updated DepotReturnTarget every x seconds (equal to seeker update rate)
		if(IsInRange(DepotReturnTarget.gameObject)) {
			// In range, deposit resources in DepotReturnTarget
			MoveTaskScript.StopTask();
			DepotReturnTarget.DepositResource(HeldResource.resource, HeldResource.amount);
			HeldResource = null;
			DepotReturnTarget = null;
		} else {
			// Not in range, make sure we're moving toward DepotReturnTarget
			MoveTaskScript.StartTask(DepotReturnTarget.transform);
		}
	}
	
	public void StartTask(MoveResourceJob moveResourceJob) {
		if(MoveResourceJob != moveResourceJob) {
			MoveResourceJob = moveResourceJob;
			// Return any other incorrect held resources to a depot before beginning
			if(!CanHoldMoreOfResource(MoveResourceJob.Resource)) {
				StartTask();
			}
		}
	}
	
	// Starts this task as a special case where all currently-held resources will be moved to the nearest depot
	public void StartTask() {
		if(HeldResource != null) {
			DepotReturnTarget = ResourceDepot.FindNearestDepotForResource(transform.position, Controllable.owner, HeldResource.resource);
		}
	}
	
	public bool IsTaskRunning() {
		return MoveResourceJob != null;
	}
	
	public void StopTask() {
		MoveResourceJob = null;
		DepotReturnTarget = null;
	}
	
	// Holds the given amount of the given resource. The hold is only successful if the same or no resource is already held
	public void HoldResource(Resource resource, int amount) {
		if(HeldResource == null) {
			HeldResource = new ResourceAmount();
			HeldResource.resource = resource;
			HeldResource.amount = amount;
		} else if(HeldResource.resource == resource) {
			HeldResource.amount += amount;
		}
		HeldResource.amount = Mathf.Clamp(HeldResource.amount, 0, heldResourceLimit);
	}
	
	public bool CanHoldMoreOfResource(Resource resource) {
		return HeldResource == null || (HeldResource.resource == resource && HeldResource.amount < heldResourceLimit);
	}
		
	public bool HoldingRequiredResourceAmount() {
		return HeldResource != null && HeldResource.resource == MoveResourceJob.Resource && HeldResource.amount >= MoveResourceJob.Amount;
	}
	
	protected bool IsInRange(GameObject gameObject) {
		float range = this.collider.bounds.size.magnitude/2 + gameObject.collider.bounds.size.magnitude/2 + 0.5f;
		return (gameObject.transform.position - this.transform.position).magnitude <= range;
	}
}
