using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AIBuilder))]
[RequireComponent(typeof(AIGatherer))]
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
			AddAction(new GatherAction(this, hit.collider.gameObject.GetComponent<ResourceNode>()), IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			AddAction(new BuildAction(this, hit.collider.gameObject.GetComponent<BuildProgressControl>()), IsMultiKeyPressed());
		} 
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		base.KeyPressed();
		
		if(queuedBuildTarget != null && Input.GetKeyDown(KeyCode.Escape)) {
			Destroy(queuedBuildTarget.gameObject);
			queuedBuildTarget = null;
		} else if(buildMenuOpen) {
			// See if pressed key exists in buildings and if so, queue the BuildProgress object for that building
			foreach(Creatable building in buildings) {
				if(Input.GetKeyDown(building.creationKey)) {
					if(building.CanCreate(owner)) {
						InstantiateBuildProgress(building);
					}
				}
			}
		} else if(Input.GetKeyDown(KeyCode.B)) {
			buildMenuOpen = true;
		}
	}
	
	protected void InstantiateBuildProgress(Creatable building) {
		queuedBuildTarget = (Game.InstantiateControllable(building.buildProgressObject, owner, Vector3.zero)).GetComponent<BuildProgressControl>();
		queuedBuildTarget.name = building.gameObject.name+" (in progress)";
	}
	
	//TODO move all this queued building stuff out of this control
	//TODO turn on some grid while placing buildings
	
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
		//TODO finish drawing queued buildings (can't intersect other objects, etc.)
	}
	
	// Commits the currently queued building at its current position and begins building
	protected void CommitQueuedBuilding() {
		if(queuedBuildTarget.creatable.CanCreate(owner)) {
			queuedBuildTarget.Commit(); //TODO commiting multiple queued buildings is clearing the action queue
			AddAction(new BuildAction(this, queuedBuildTarget), IsMultiKeyPressed());
		}
		if(IsMultiKeyPressed() && queuedBuildTarget.creatable.CanCreate(owner)) {
			InstantiateBuildProgress(queuedBuildTarget.creatable);
		} else {
			queuedBuildTarget = null;
		}
	}
	
	// Called when this GameObject has been deselected
	public override void Deselected() {
		buildMenuOpen = false;
		if(queuedBuildTarget != null) {
			Destroy(queuedBuildTarget.gameObject);
			queuedBuildTarget = null;
		}
	}
}
