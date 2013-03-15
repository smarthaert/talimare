using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AIBuilder))]
[RequireComponent(typeof(AIGatherer))]
public class CivilianControl : BaseUnitControl {
	
	// Buildings this unit can build
	public List<Creatable> buildings;
	//TODO ! make building templates like unit templates
	
	protected bool buildMenuOpen = false;
	protected BuildProgress queuedBuildTarget;
	
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
		
		//TODO queue gather and build actions
		if(hit.collider.gameObject.CompareTag("Resource")) {
			SendMessage("StopAllActions");
			//Gather(hit.collider.gameObject.GetComponent<ResourceNode>());
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			SendMessage("StopAllActions");
			//Build(hit.collider.gameObject.GetComponent<BuildProgress>());
		} else if(queuedBuildTarget != null) {
			SendMessage("StopAllActions");
			CommitQueuedBuilding(hit.point);
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		base.KeyPressed();
		if(buildMenuOpen) {
			// See if pressed key exists in buildings and if so, queue the BuildProgress object for that building, and also give it a Player
			foreach(Creatable building in buildings) {
				if(Input.GetKeyDown(building.creationKey)) {
					if(building.CanCreate()) {
						queuedBuildTarget = ((GameObject)Instantiate(building.buildProgressObject.gameObject)).GetComponent<BuildProgress>();
						queuedBuildTarget.owner = owner;
					}
				}
			}
		} else if(Input.GetKeyDown(KeyCode.B)) {
			buildMenuOpen = !buildMenuOpen;
		}
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
	
	// Commits the currently queued building at the given position and begins building
	protected void CommitQueuedBuilding(Vector3 position) {
		if(queuedBuildTarget.creatable.CanCreate()) {
			queuedBuildTarget.Commit();
			//TODO build action
			//Build(queuedBuildTarget);
		}
		queuedBuildTarget = null;
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
