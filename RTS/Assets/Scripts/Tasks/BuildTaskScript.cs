using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(MoveTaskScript))]
public class BuildTaskScript : MonoBehaviour {
	
	// The current job this unit is tasked to complete
	protected BuildJob BuildJob { get; set; }
	protected bool HasStartedBuilding { get; set; }
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if(BuildJob != null) {
			if(BuildJob.Completed) {
				StopTask();
			} else {
				UpdateBuild();
			}
		}
	}
	
	protected void UpdateBuild() {
		if(HasStartedBuilding) {
			// Currently building
			BuildJob.AdvanceBuildCompletion(Time.deltaTime);
		} else {
			// Not building yet due to being out of range
			//TODO ensure all buildjob sub jobs are completed
			if(IsInBuildRange()) {
				MoveTaskScript.StopTask();
				HasStartedBuilding = true;
			} else {
				MoveTaskScript.StartTask(BuildJob.BuildTarget.transform);
			}
		}
	}
	
	public void StartTask(BuildJob buildJob) {
		if(BuildJob != buildJob) {
			BuildJob = buildJob;
			HasStartedBuilding = false;
		}
	}
	
	public bool IsTaskRunning() {
		return BuildJob != null;
	}
	
	public void StopTask() {
		BuildJob = null;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + BuildJob.BuildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (BuildJob.BuildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
