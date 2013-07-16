using UnityEngine;
using System.Collections.Generic;

// Handles unit building
[RequireComponent(typeof(Controllable))]
[RequireComponent(typeof(MoveTaskScript))]
[AddComponentMenu("Tasks/Build")]
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
	
	protected void BuildControlMenus() {
		Controllable.ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BUILDINGS, ControlStore.MENU_BUILDINGS));
		
		ControlMenu createBuildingMenu = new ControlMenu();
		foreach(CreatableBuilding building in Game.PlayerInput.buildings) {
			createBuildingMenu.MenuItems.Add(new ControlMenuItem(building, ControlStore.MENU_CANCEL));
		}
		createBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		Controllable.ControlMenus.Add(ControlStore.MENU_BUILDINGS, createBuildingMenu);
		
		ControlMenu cancelCreateMenu = new ControlMenu();
		cancelCreateMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CANCEL, ControlStore.MENU_BUILDINGS));
		Controllable.ControlMenus.Add(ControlStore.MENU_CANCEL, cancelCreateMenu);
	}
	
	public void ReceiveMouseAction(RaycastHit hit) {
		if(hit.transform.CompareTag(GameUtil.TAG_BUILD_PROGRESS)) {
			hit.transform.GetComponent<BuildProgressControl>().BuildJob.AssignNextJob(Controllable, Game.PlayerInput.IsMultiKeyPressed());
		}
	}
	
	public void ReceiveControlCode(string controlCode) {
		if(controlCode.Equals(ControlStore.MENU_CANCEL)) {
			Game.PlayerInput.RemoveQueuedBuildTarget(true);
		} else {
			// See if control code exists in buildings and if so, queue the BuildProgress object for that building
			foreach(CreatableBuilding building in Game.PlayerInput.buildings) {
				if(building.ControlCode.Equals(controlCode) && building.CanCreate(Game.ThisPlayer).Bool) {
					Game.PlayerInput.InstantiateBuildProgress(building);
				}
			}
		}
	}
	
	protected void Update() {
		if(BuildJob != null) {
			if(BuildJob.Completed || BuildJob.BuildTarget == null) {
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
			if(!BuildJob.AllSubJobsComplete) {
				Debug.LogError("All build sub jobs are not complete! We shouldn't be building yet.");
			}
		}
	}
	
	public bool IsTaskRunning() {
		return BuildJob != null;
	}
	
	public void StopTask() {
		if(BuildJob != null) {
			BuildJob.RemoveAssignee(Controllable);
			BuildJob = null;
		}
		MoveTaskScript.StopTask();
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + BuildJob.BuildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (BuildJob.BuildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
