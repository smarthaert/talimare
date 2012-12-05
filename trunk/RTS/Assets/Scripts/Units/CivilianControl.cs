using UnityEngine;
using System.Collections.Generic;

public class CivilianControl : UnitControl {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	// The resource node this unit is currently gathering
	protected ResourceNode gatherTarget;
	protected float gatherTimer = 0;
	
	// Buildings this unit can build
	public List<Creatable> buildings;
	
	// The building this unit is currently building
	protected bool buildMenuOpen = false;
	protected BuildProgress queuedBuildTarget;
	protected BuildProgress buildTarget;
	protected bool isBuilding = false;
	
	protected static int? terrainLayer;

	protected override void Start () {
		base.Start();
		if(terrainLayer == null)
			terrainLayer = GameObject.Find("Terrain").layer;
	}
	
	protected override void Update () {
		base.Update();
		
		if(gatherTarget != null) {
			UpdateGather();
		} else if(queuedBuildTarget != null) {
			DrawQueuedBuildingAtMouse();
		} else if(buildTarget != null) {
			UpdateBuild();
		}
	}
	
	protected void UpdateGather() {
		if(gatherTimer > 0) {
			// Currently gathering
			gatherTimer -= Time.deltaTime;
			if(gatherTimer <= 0) {
				// Timer's up, trigger gather from node if still in range
				if(IsInGatherRange()) {
					gatherTarget.Gather(gatherAmount);
					player.playerStatus.GainResource(gatherTarget.resource, gatherAmount);
					gatherTimer = gatherTime;
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInGatherRange()) {
				// In range, start gathering
				pathfinder.StopMoving();
				gatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				pathfinder.Move(gatherTarget.transform);
			}
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
	
	protected void UpdateBuild() {
		if(isBuilding) {
			// Currently building
			buildTarget.Building(Time.deltaTime);
			if(buildTarget.Completed()) {
				StopBuilding();
			}
		} else {
			// Haven't started building yet
			if(IsInBuildRange()) {
				pathfinder.StopMoving();
				isBuilding = true;
			} else {
				pathfinder.Move(buildTarget.transform);
			}
		}
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		base.MouseAction(hit);
		if(hit.collider.gameObject.CompareTag("Resource")) {
			SendMessage("StopAllActions");
			Gather(hit.collider.gameObject.GetComponent<ResourceNode>());
		} else if(queuedBuildTarget != null) {
			SendMessage("StopAllActions");
			CommitQueuedBuilding(hit.point);
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			SendMessage("StopAllActions");
			Build(hit.collider.gameObject.GetComponent<BuildProgress>());
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		base.KeyPressed();
		if(buildMenuOpen) {
			// See if pressed key exists in buildings and if so, queue the BuildProgress object for that building, and also give it a Player
			foreach(Creatable building in buildings) {
				if(Input.GetKeyDown(building.creationKey)) {
					if(building.CanCreate(player)) {
						queuedBuildTarget = ((GameObject)Instantiate(building.buildProgressObject.gameObject)).GetComponent<BuildProgress>();
						queuedBuildTarget.player = player;
					}
				}
			}
		} else if(Input.GetKeyDown(KeyCode.B)) {
			buildMenuOpen = !buildMenuOpen;
		}
	}
	
	// Sets this unit to gather from the given resource node
	public void Gather(ResourceNode node) {
		if(node != gatherTarget) {
			gatherTimer = 0;
			gatherTarget = node;
		}
	}
	
	// Commits the currently queued building at the given position and begins building
	protected void CommitQueuedBuilding(Vector3 position) {
		if(queuedBuildTarget.creatable.CanCreate(player)) {
			queuedBuildTarget.Commit();
			Build(queuedBuildTarget);
		}
		queuedBuildTarget = null;
	}
	
	// Sets this unit to build the given building
	public void Build(BuildProgress buildProgress) {
		if(buildProgress != buildTarget) {
			buildTarget = buildProgress;
		}
	}
	
	public void StopGathering() {
		gatherTarget = null;
	}
	
	public void StopBuilding() {
		buildTarget = null;
		isBuilding = false;
	}
	
	// Stops all actions the unit is performing. Keep in mind that it's likely that one of the stopped actions will be resumed immediately
	public override void StopAllActions() {
		base.StopAllActions();
		StopGathering();
		StopBuilding();
	}
	
	// Called when this GameObject has been deselected
	public override void Deselected() {
		buildMenuOpen = false;
		if(queuedBuildTarget != null) {
			Destroy(queuedBuildTarget.gameObject);
			queuedBuildTarget = null;
		}
	}
	
	protected bool IsInGatherRange() {
		float gatherRange = this.collider.bounds.size.magnitude/2 + gatherTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (gatherTarget.transform.position - this.transform.position).magnitude <= gatherRange;
	}
	
	protected bool IsInBuildRange() {
		float buildRange = this.collider.bounds.size.magnitude/2 + buildTarget.collider.bounds.size.magnitude/2 + 0.5f;
		return (buildTarget.transform.position - this.transform.position).magnitude <= buildRange;
	}
}
