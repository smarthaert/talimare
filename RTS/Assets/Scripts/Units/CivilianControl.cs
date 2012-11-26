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
	protected Creatable queuedBuilding;
	protected BuildProgress buildTarget;
	protected bool isBuilding = false;
	
	protected static PlayerStatus playerStatus;

	protected override void Start () {
		base.Start();
		
		if(playerStatus == null)
			playerStatus = GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
	}
	
	protected override void Update () {
		base.Update();
		
		if(gatherTarget != null) {
			UpdateGather();
		}
		if(buildTarget != null) {
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
					playerStatus.GainResource(gatherTarget.resource, gatherAmount);
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
			attacker.StopAttacking();
			StopBuilding();
			Gather(hit.collider.gameObject.GetComponent<ResourceNode>());
		} else if(queuedBuilding != null && hit.collider.GetType() == typeof(TerrainCollider)) {
			attacker.StopAttacking();
			StopBuilding();
			commitQueuedBuilding(hit.point);
		} else if(hit.collider.gameObject.CompareTag("BuildProgress")) {
			attacker.StopAttacking();
			StopGathering();
			Build(hit.collider.gameObject.GetComponent<BuildProgress>());
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		base.KeyPressed();
		if(buildMenuOpen) {
			// See if pressed key exists in buildings and if so, queue that Creatable
			foreach(Creatable building in buildings) {
				if(Input.GetKeyDown(building.creationKey)) {
					if(building.CanCreate()) {
						queuedBuilding = building;
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
	protected void commitQueuedBuilding(Vector3 position) {
		if(queuedBuilding.CanCreate()) {
			queuedBuilding.SpendResources();
			GameObject newBuilding = (GameObject)Instantiate(queuedBuilding.buildProgressObject.gameObject, position, Quaternion.identity);
			Build(newBuilding.GetComponent<BuildProgress>());
		}
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
	
	public override void StopAllActions() {
		StopGathering();
		StopBuilding();
	}
	
	// Called when this GameObject has been deselected
	public override void Deselected() {
		buildMenuOpen = false;
		queuedBuilding = null;
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
