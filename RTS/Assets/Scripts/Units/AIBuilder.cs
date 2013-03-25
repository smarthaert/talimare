using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(AIPathfinder))]
public class AIBuilder : ActionScript {
	
	// The building this unit is currently building
	protected BuildProgressControl buildTarget;
	protected bool hasStartedBuilding = false;
	
	protected AIPathfinder pathfinder;
	
	protected void Awake() {
		pathfinder = GetComponent<AIPathfinder>();
	}
	
	protected void Update () {
		if(buildTarget != null) {
			UpdateBuild();
		}
	}
	
	protected void UpdateBuild() {
		if(hasStartedBuilding) {
			// Currently building
			buildTarget.Building(Time.deltaTime);
			if(buildTarget.Completed()) {
				StopAction();
			}
		} else {
			// Haven't started building yet
			if(IsInBuildRange()) {
				pathfinder.StopMoving();
				hasStartedBuilding = true;
			} else {
				pathfinder.Move(buildTarget.transform);
			}
		}
	}
	
	public override void StartAction(object target) {
		buildTarget = (BuildProgressControl)target;
		hasStartedBuilding = false;
	}
	
	public override bool IsActing() {
		return buildTarget != null;
	}
	
	public override void StopAction() {
		buildTarget = null;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + buildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (buildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
