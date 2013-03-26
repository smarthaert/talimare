using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(AIPathfinder))]
public class AIBuilder : ActionScript {
	
	// The building this unit is currently building
	protected BuildProgressControl BuildTarget { get; set; }
	protected bool HasStartedBuilding { get; set; }
	
	protected AIPathfinder Pathfinder { get; set; }
	
	protected void Awake() {
		Pathfinder = GetComponent<AIPathfinder>();
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
			if(BuildTarget.Completed()) {
				StopAction();
			}
		} else {
			// Haven't started building yet
			if(IsInBuildRange()) {
				Pathfinder.StopAction();
				HasStartedBuilding = true;
			} else {
				Pathfinder.StartAction(BuildTarget.transform);
			}
		}
	}
	
	public override void StartAction(object target) {
		if(BuildTarget != (BuildProgressControl)target) {
			BuildTarget = (BuildProgressControl)target;
			HasStartedBuilding = false;
		}
	}
	
	public override bool IsActing() {
		return BuildTarget != null;
	}
	
	public override void StopAction() {
		BuildTarget = null;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + BuildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (BuildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
