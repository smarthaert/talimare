using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BuildTaskScript))]
[RequireComponent(typeof(GatherTaskScript))]
public class CivilianControl : BaseUnitControl {
	
	// Buildings this unit can build
	public List<Creatable> buildings;
	
	protected BuildProgressControl queuedBuildTarget;
	
	protected static int? terrainLayer;

	protected override void Start () {
		base.Start();
		
		if(terrainLayer == null)
			terrainLayer = GameObject.Find("Terrain").layer;
	}
	
	protected override void BuildControlMenus() {
		base.BuildControlMenus();
		
		ControlMenu baseUnitMenu = ControlMenus[0];
		baseUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BUILDINGS, "createBuilding"));
		
		ControlMenu createBuildingMenu = new ControlMenu("createBuilding");
		foreach(Creatable building in buildings) {
			createBuildingMenu.MenuItems.Add(new ControlMenuItem(building, "cancelCreate"));
		}
		createBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, "baseUnit"));
		ControlMenus.Add(createBuildingMenu);
		
		ControlMenu cancelCreateMenu = new ControlMenu("cancelCreate");
		cancelCreateMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CANCEL, "createBuilding"));
		ControlMenus.Add(cancelCreateMenu);
	}
	
	protected override void Update () {
		base.Update();
		
		if(queuedBuildTarget != null) {
			DrawQueuedBuildingAtMouse();
		}
	}
	
	public override void MouseAction(RaycastHit hit) {
		base.MouseAction(hit);
		
		if(queuedBuildTarget != null) {
			CommitQueuedBuilding();
		} else if(hit.collider.gameObject.CompareTag("Resource")) {
			AddTask(new Task(GetComponent<GatherTaskScript>(), hit.collider.gameObject.GetComponent<ResourceNode>()), Game.PlayerInput.IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			AddTask(new Task(GetComponent<BuildTaskScript>(), hit.collider.gameObject.GetComponent<BuildProgressControl>()), Game.PlayerInput.IsMultiKeyPressed());
		} 
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.MENU_CANCEL)) {
			RemoveQueuedBuildTarget(true);
		} else {
			// See if control code exists in buildings and if so, queue the BuildProgress object for that building
			foreach(Creatable building in buildings) {
				if(building.ControlCode.Equals(controlCode) && building.CanCreate(owner).Bool) {
					InstantiateBuildProgress(building);
				}
			}
		}
	}
	
	protected void InstantiateBuildProgress(Creatable building) {
		queuedBuildTarget = (GameUtil.InstantiateControllable(building.buildProgressObject, owner, Vector3.zero)).GetComponent<BuildProgressControl>();
		queuedBuildTarget.name = building.gameObject.name+" (in progress)";
	}
	
	//TODO low: turn on some grid while placing buildings?
	
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
		//TODO low: finish drawing queued buildings (can't intersect other objects, etc.)
	}
	
	// Commits the currently queued building at its current position and begins building
	protected void CommitQueuedBuilding() {
		if(queuedBuildTarget.Creatable.CanCreate(owner).Bool) {
			queuedBuildTarget.Commit();
			AddTask(new Task(GetComponent<BuildTaskScript>(), queuedBuildTarget), Game.PlayerInput.IsMultiKeyPressed());
		}
		if(Game.PlayerInput.IsMultiKeyPressed() && queuedBuildTarget.Creatable.CanCreate(owner).Bool) {
			InstantiateBuildProgress(queuedBuildTarget.Creatable);
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
			CurrentControlMenu = ControlMenus[0];
		}
	}
}
