using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BuildTaskScript))]
[RequireComponent(typeof(GatherTaskScript))]
public class CivilianControl : BaseUnitControl {
	
	// Buildings this unit can build
	public List<Creatable> buildings;
	
	protected bool buildMenuOpen = false;
	protected BuildProgressControl queuedBuildTarget;
	
	protected static int? terrainLayer;

	protected override void Start () {
		base.Start();
		
		if(terrainLayer == null)
			terrainLayer = GameObject.Find("Terrain").layer;
	}
	
	protected override void Update () {
		base.Update();
		
		if(queuedBuildTarget != null) {
			DrawQueuedBuildingAtMouse();
		}
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		base.MouseAction(hit);
		
		if(queuedBuildTarget != null) {
			CommitQueuedBuilding();
		} else if(hit.collider.gameObject.CompareTag("Resource")) {
			AddTask(new Task(GetComponent<GatherTaskScript>(), hit.collider.gameObject.GetComponent<ResourceNode>()), IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			AddTask(new Task(GetComponent<BuildTaskScript>(), hit.collider.gameObject.GetComponent<BuildProgressControl>()), IsMultiKeyPressed());
		} 
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		base.KeyPressed();
		
		if(queuedBuildTarget != null) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				RemoveQueuedBuildTarget(true);
			}
		} else {
			if(buildMenuOpen) {
				// See if pressed key exists in buildings and if so, queue the BuildProgress object for that building
				foreach(Creatable building in buildings) {
					if(Input.GetKeyDown(building.creationKey) && building.CanCreate(owner)) {
						InstantiateBuildProgress(building);
					}
				}
			} else if(Input.GetKeyDown(KeyCode.B)) {
				buildMenuOpen = true;
			}
		}
	}
	
	protected void InstantiateBuildProgress(Creatable building) {
		queuedBuildTarget = (GameUtil.InstantiateControllable(building.buildProgressObject, owner, Vector3.zero)).GetComponent<BuildProgressControl>();
		queuedBuildTarget.name = building.gameObject.name+" (in progress)";
		Game.PlayerInput.DeselectDisabled = true;
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
		if(queuedBuildTarget.Creatable.CanCreate(owner)) {
			queuedBuildTarget.Commit();
			AddTask(new Task(GetComponent<BuildTaskScript>(), queuedBuildTarget), IsMultiKeyPressed());
		}
		if(IsMultiKeyPressed() && queuedBuildTarget.Creatable.CanCreate(owner)) {
			InstantiateBuildProgress(queuedBuildTarget.Creatable);
		} else {
			RemoveQueuedBuildTarget(false);
		}
	}
	
	// Called when this GameObject has been deselected
	public override void Deselected() {
		buildMenuOpen = false;
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
		}
		Game.PlayerInput.DeselectDisabled = false;
	}
}