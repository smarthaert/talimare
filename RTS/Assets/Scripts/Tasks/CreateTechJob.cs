using UnityEngine;
using System.Collections.Generic;

public class CreateTechJob : Job {
	public BuildingCommonControl Building { get; protected set; }
	public CreatableTech Tech { get; protected set; }
	
	public bool CreationStarted { get; set; }
	public float CreationTime { get; protected set; }
	
	public bool ReadyForCreationStart {
		get {
			if(AllSubJobsComplete) {
				// Since nobody actually needs to take this job, we can remove it from the owning StrategicAI as soon as all sub jobs are complete
				Owner.StrategicAI.JobComplete(this);
				return true;
			} else {
				return false;
			}
		}
	}
	public override bool Completed {
		get {
			return CreationStarted && CreationTime >= Tech.creationTime;
		}
	}
	
	public CreateTechJob(BuildingCommonControl building, CreatableTech tech, Player owner, bool isRootJob) : base(owner, isRootJob) {
		Building = building;
		Tech = tech;
		CreationStarted = false;
		CreationTime = 0f;
		foreach(ResourceAmount resourceAmount in Tech.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource()) {
				AddSubJob(new MoveResourceJob(resourceAmount.resource, resourceAmount.amount, building, owner, false));
			}
		}
	}
	
	protected override bool CanTakeThisJob(Controllable assignee) {
		return false;
	}
	
	public void AdvanceCreationTime(float deltaTime) {
		CreationTime += deltaTime;
	}
}