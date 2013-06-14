using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BuildTaskScript))]
[RequireComponent(typeof(GatherTaskScript))]
[AddComponentMenu("Units/Civilian Control")]
public class CivilianControl : BaseUnitControl {
	
	// Buildings this unit can build
	public List<CreatableBuilding> buildings;
	
	protected BuildProgressControl queuedBuildTarget;
	
	protected static int? terrainLayer;

	protected override void Start() {
		base.Start();
		
		if(terrainLayer == null) {
			terrainLayer = GameObject.Find("Terrain").layer;
		}
	}
	//TODO control menus should be built inside task scripts
	protected override void BuildControlMenus() {
		base.BuildControlMenus();
		
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BUILDINGS, ControlStore.MENU_BUILDINGS));
		
		ControlMenu createBuildingMenu = new ControlMenu();
		foreach(CreatableBuilding building in buildings) {
			createBuildingMenu.MenuItems.Add(new ControlMenuItem(building, ControlStore.MENU_CANCEL));
		}
		createBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		ControlMenus.Add(ControlStore.MENU_BUILDINGS, createBuildingMenu);
		
		ControlMenu cancelCreateMenu = new ControlMenu();
		cancelCreateMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CANCEL, ControlStore.MENU_BUILDINGS));
		ControlMenus.Add(ControlStore.MENU_CANCEL, cancelCreateMenu);
	}
	
	protected override void Update() {
		base.Update();
		
		if(queuedBuildTarget != null) {
			DrawQueuedBuildingAtMouse();
		}
	}
	
	public override void ReceiveMouseAction(RaycastHit hit) {
		base.ReceiveMouseAction(hit);
		
		if(queuedBuildTarget != null) {
			CommitQueuedBuilding();
		} else if(hit.collider.gameObject.CompareTag(GameUtil.TAG_BUILD_PROGRESS)) {
			hit.collider.gameObject.GetComponent<BuildProgressControl>().BuildJob.AssignNextJob(this, Game.PlayerInput.IsMultiKeyPressed());
		}
	}
	//TODO mouse action & receive control code should move into task scripts
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.MENU_CANCEL)) {
			RemoveQueuedBuildTarget(true);
		} else {
			// See if control code exists in buildings and if so, queue the BuildProgress object for that building
			foreach(CreatableBuilding building in buildings) {
				if(building.ControlCode.Equals(controlCode) && building.CanCreate(Owner).Bool) {
					InstantiateBuildProgress(building);
				}
			}
		}
	}
	
	protected void InstantiateBuildProgress(CreatableBuilding building) {
		queuedBuildTarget = (GameUtil.InstantiateControllable(building.buildProgressControl, Owner, Vector3.zero)).GetComponent<BuildProgressControl>();
		queuedBuildTarget.name = building.gameObject.name+" (in progress)";
	}
	
	// Moves the queued building to where the mouse hits the ground
	protected void DrawQueuedBuildingAtMouse() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << terrainLayer.Value))) {
			// Shift the target position up on top of the ground and snap it to grid
			Vector3 position = hit.point;
			position.y += (queuedBuildTarget.renderer.bounds.size.y / 2);
			position.x = Mathf.RoundToInt(position.x);
			position.y = Mathf.RoundToInt(position.y);
			position.z = Mathf.RoundToInt(position.z);
			queuedBuildTarget.transform.position = position;
		}
	}
	
	// Commits the currently queued building at its current position and immediately tasks this unit on its BuildJob
	protected void CommitQueuedBuilding() {
		if(queuedBuildTarget.FinishedBuildingCreatable.CanCreate(Owner).Bool) {
			queuedBuildTarget.Commit();
			queuedBuildTarget.BuildJob.AssignNextJob(this, Game.PlayerInput.IsMultiKeyPressed());
		}
		if(Game.PlayerInput.IsMultiKeyPressed() && queuedBuildTarget.FinishedBuildingCreatable.CanCreate(Owner).Bool) {
			InstantiateBuildProgress(queuedBuildTarget.FinishedBuildingCreatable);
		} else {
			RemoveQueuedBuildTarget(false);
		}
	}
	
	public override void Deselected() {
		base.Deselected();
		
		if(queuedBuildTarget != null) {
			RemoveQueuedBuildTarget(true);
		}
	}
	
	// Removes the currently queued build target reference. If true is passed, the target will be completely destroyed
	protected void RemoveQueuedBuildTarget(bool andDestroyIt) {
		if(andDestroyIt) {
			Destroy(queuedBuildTarget.gameObject);
		} else {
			queuedBuildTarget = null;
			CurrentControlMenu = ControlMenus[ControlStore.MENU_BASE];
		}
	}
}
