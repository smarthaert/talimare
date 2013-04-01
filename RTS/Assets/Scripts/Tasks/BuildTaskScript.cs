using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(MoveTaskScript))]
public class BuildTaskScript : TaskScript {
	
	// The building this unit is currently building
	protected BuildProgressControl BuildTarget { get; set; }
	protected bool HasStartedBuilding { get; set; }
	
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
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
			if(BuildTarget.Completed()) {
				StopTask();
			}
		} else {
			// Haven't started building yet
			if(IsInBuildRange()) {
				MoveTaskScript.StopTask();
				HasStartedBuilding = true;
			} else {
				MoveTaskScript.StartTask(BuildTarget.transform);
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
