using UnityEngine;
using System.Collections.Generic;

public class MoveResourceJob : Job {
	// Resource type to be moved
	public Resource Resource { get; protected set; }
	// Amount of resource to be moved
	public int Amount { get; protected set; }
	// Destination for the moved resources
	public BuildProgressControl Destination { get; protected set; }
	
	protected int MovedAmount { get; set; }
	public int AmountRemaining { get { return Amount - MovedAmount; } }
	
	public override bool Completed {
		get {
			return MovedAmount >= Amount;
		}
	}
	
	public MoveResourceJob(Resource resource, int amount, BuildProgressControl destination, Player owner) : base(owner) {
		Resource = resource;
		Amount = amount;
		Destination = destination;
		MovedAmount = 0;
	}
	
	protected override bool CanTakeThisJob(Controllable assignee) {
		return Assignees.Count == 0 && assignee.GetComponent<MoveResourceTaskScript>() != null && ResourceDepot.FindAllDepotsWithResource(Resource, Owner).Count > 0;
	}

	protected override void AssignThisJob(Controllable assignee, bool? appendToTaskQueue) {
		base.AssignThisJob(assignee, appendToTaskQueue);
		
		if(!appendToTaskQueue.HasValue) {
			assignee.AddTaskInterruptAfterCurrent(new MoveResourceTask(assignee.GetComponent<MoveResourceTaskScript>(), this));
		} else {
			assignee.AddTask(new MoveResourceTask(assignee.GetComponent<MoveResourceTaskScript>(), this), appendToTaskQueue.Value);
		}
	}
	
	// Stores the given amount of Resource in Destination
	public void MoveResource(int amount) {
		Destination.StoredResources[Resource] += amount;
		MovedAmount += amount;
	}
}