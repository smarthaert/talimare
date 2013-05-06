using UnityEngine;
using System.Collections.Generic;

public class MoveResourceJob : Job {
	public Resource Resource { get; protected set; }
	public int Amount { get; protected set; }
	public BuildProgressControl Destination { get; protected set; }
	
	public MoveResourceJob(Resource resource, int amount, BuildProgressControl destination) {
		Resource = resource;
		Amount = amount;
		Destination = destination;
	}
	
	protected override bool CanTakeThisJob(Controllable jobTaker) {
		return Assignees.Count == 0 && jobTaker.GetComponent<MoveResourceTaskScript>() != null;
	}

	protected override void AssignThisJob(Controllable jobTaker, bool appendToTaskQueue) {
		base.AssignThisJob(jobTaker, appendToTaskQueue);
		
		jobTaker.AddTask(new MoveResourceTask(jobTaker.GetComponent<MoveResourceTaskScript>(), this), appendToTaskQueue);
	}
}