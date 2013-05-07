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
	
	public MoveResourceJob(Resource resource, int amount, BuildProgressControl destination) {
		Resource = resource;
		Amount = amount;
		Destination = destination;
		MovedAmount = 0;
	}
	
	protected override bool CanTakeThisJob(Controllable jobTaker) {
		return Assignees.Count == 0 && jobTaker.GetComponent<MoveResourceTaskScript>() != null;
	}

	protected override void AssignThisJob(Controllable jobTaker, bool appendToTaskQueue) {
		base.AssignThisJob(jobTaker, appendToTaskQueue);
		
		jobTaker.AddTask(new MoveResourceTask(jobTaker.GetComponent<MoveResourceTaskScript>(), this), appendToTaskQueue);
	}
	
	// Stores the given amount of Resource in Destination
	public void MoveResource(int amount) {
		Destination.StoredResources.Add(Resource, amount);
		MovedAmount += amount;
	}
}