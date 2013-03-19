using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(AIPathfinder))]
public class AIBuilder : MonoBehaviour {
	
	// The building this unit is currently building
	protected BuildProgressControl buildTarget;
	protected bool hasStartedBuilding = false;
	
	protected AIPathfinder pathfinder;
	
	protected void Start() {
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
				StopBuilding();
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
	
	// Sets this unit to build the given building
	public void Build(BuildProgressControl buildProgress) {
		if(buildProgress != buildTarget) {
			buildTarget = buildProgress;
		}
	}
	
	public void StopBuilding() {
		buildTarget = null;
		hasStartedBuilding = false;
	}
	
	public bool IsBuilding() {
		return buildTarget == null;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + buildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (buildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
