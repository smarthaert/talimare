using UnityEngine;
using System.Collections.Generic;

public class BuildJob : Job {
	public BuildProgressControl BuildTarget { get; protected set; }
	
	public override bool Completed {
		get {
			return BuildTarget.Completed;
		}
	}
	
	public BuildJob(BuildProgressControl buildTarget, Player owner, bool isRootJob) : base(owner, isRootJob) {
		BuildTarget = buildTarget;
		foreach(ResourceAmount resourceAmount in BuildTarget.FinishedBuildingCreatable.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource()) {
				AddSubJob(new MoveResourceJob(resourceAmount.resource, resourceAmount.amount, buildTarget, owner, false));
			}
		}
	}
	
	protected override bool CanTakeThisJob(Controllable assignee) {
		return assignee.GetComponent<BuildTaskScript>() != null;
	}

	protected override void AssignThisJob(Controllable assignee, bool? appendToTaskQueue) {
		base.AssignThisJob(assignee, appendToTaskQueue);
		
		if(!appendToTaskQueue.HasValue) {
			assignee.AddTaskInterruptAfterCurrent(new BuildTask(assignee.GetComponent<BuildTaskScript>(), this));
		} else {
			assignee.AddTask(new BuildTask(assignee.GetComponent<BuildTaskScript>(), this), appendToTaskQueue.Value);
		}
	}
	
	public void AdvanceBuildCompletion(float timeSpent) {
		BuildTarget.Building(timeSpent);
	}
}