using UnityEngine;
using System.Collections.Generic;

public class BuildJob : Job {
	public BuildProgressControl BuildTarget { get; protected set; }
	
	public BuildJob(BuildProgressControl buildTarget) {
		BuildTarget = buildTarget;
		foreach(ResourceAmount resourceAmount in buildTarget.Creatable.resourceCosts) {
			AddSubJob(0, new MoveResourceJob(resourceAmount.resource, resourceAmount.amount, buildTarget));
		}
	}
	
	protected override bool CanTakeThisJob(Controllable jobTaker) {
		return Assignees.Count == 0 && jobTaker.GetComponent<BuildTaskScript>() != null;
	}

	protected override void AssignThisJob(Controllable jobTaker, bool appendToTaskQueue) {
		base.AssignThisJob(jobTaker, appendToTaskQueue);
		
		jobTaker.AddTask(new BuildTask(jobTaker.GetComponent<BuildTaskScript>(), this), appendToTaskQueue);
	}
}