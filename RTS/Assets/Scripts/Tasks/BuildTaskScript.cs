using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(MoveTaskScript))]
public class BuildTaskScript : TaskScript {
	
	// The building this unit is currently building
	protected BuildProgressControl BuildTarget { get; set; }
	protected bool HasStartedBuilding { get; set; }
	
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected override void Awake() {
		base.Awake();
		
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if(BuildTarget != null) {
			UpdateBuild();
		}
	}
	
	protected void UpdateBuild() {
		if(HasStartedBuilding) {
			// Currently building
			BuildTarget.Building(Time.deltaTime);
			if(BuildTarget.Completed) {
				StopTask();
			}
		} else {
			// Haven't started building yet
			ResourceAmount requiredResource = BuildTarget.GetNextResourceNeededToBuild();
			if(requiredResource == null) {
				if(IsInBuildRange()) {
					MoveTaskScript.StopTask();
					HasStartedBuilding = true;
				} else {
					MoveTaskScript.StartTask(BuildTarget.transform);
				}
			} else {
				ResourceDepot nearestDepot = ResourceDepot.FindNearestDepotWithResource(transform.position, Controllable.owner, requiredResource.resource);
				//TODO high: gather build materials from depots
			}
		}
	}
	
	public override void StartTask(object target) {
		if(BuildTarget != (BuildProgressControl)target) {
			BuildTarget = (BuildProgressControl)target;
			HasStartedBuilding = false;
		}
	}
	
	public override bool IsTaskRunning() {
		return BuildTarget != null;
	}
	
	public override void StopTask() {
		BuildTarget = null;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + BuildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (BuildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
