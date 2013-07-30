using UnityEngine;
using System.Collections.Generic;

public class CreateUnitJob : Job {
	public BuildingCommonControl Building { get; protected set; }
	protected Controllable UnitToConvert { get; set; }
	public CreatableUnit DestinationUnit { get; protected set; }
	
	public bool CreationStarted { get; set; }
	public float CreationTime { get; protected set; }
	
	public bool HasAssignee {
		get {
			return Assignees.Count > 0;
		}
	}
	public Controllable Assignee {
		get {
			if(Assignees.Count == 0)
				return null;
			else
				return Assignees[0];
		}
	}
	public bool IsConversion {
		get {
			return UnitToConvert != null;
		}
	}
	public bool ReadyForCreationStart {
		get {
			return (IsConversion && HasAssignee && AssigneeIsInCreationRange()) || (!IsConversion && AllSubJobsComplete);
		}
	}
	public override bool Completed {
		get {
			return CreationStarted && CreationTime >= DestinationUnit.creationTime;
		}
	}
	
	public CreateUnitJob(BuildingCommonControl building, Controllable unitToConvert, CreatableUnit destinationUnit, Player owner, bool isRootJob) : base(owner, isRootJob) {
		Building = building;
		UnitToConvert = unitToConvert;
		DestinationUnit = destinationUnit;
		CreationStarted = false;
		CreationTime = 0f;
		foreach(ResourceAmount resourceAmount in DestinationUnit.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource()) {
				AddSubJob(new MoveResourceJob(resourceAmount.resource, resourceAmount.amount, building, owner, false));
			}
		}
	}
	
	protected override bool CanTakeThisJob(Controllable assignee) {
		return IsConversion && Assignees.Count == 0 && assignee == UnitToConvert;
	}

	protected override void AssignThisJob(Controllable assignee, bool? appendToTaskQueue) {
		base.AssignThisJob(assignee, appendToTaskQueue);
		
		if(!appendToTaskQueue.HasValue) {
			assignee.AddTaskInterruptAfterCurrent(new MoveTask(assignee.GetComponent<MoveTaskScript>(), Building.transform));
		} else {
			assignee.AddTask(new MoveTask(assignee.GetComponent<MoveTaskScript>(), Building.transform), appendToTaskQueue.Value);
		}
	}
	
	public void AdvanceCreationTime(float deltaTime) {
		CreationTime += deltaTime;
	}
	
	protected bool AssigneeIsInCreationRange() {
		float range = Assignee.collider.bounds.size.magnitude/2 + Building.collider.bounds.size.magnitude/2 + 0.5f;
		return (Building.transform.position - Assignee.transform.position).magnitude <= range;
	}
}