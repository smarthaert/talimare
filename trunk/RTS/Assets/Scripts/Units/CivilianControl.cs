using UnityEngine;
using System.Collections;

public class CivilianControl : UnitControl {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Range from which this unit can gather a resource node
	protected float gatherRange = 2;
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	//TODO add a list of buildable buildings
	
	// Range from which this unit can build
	protected float buildRange = 4;
	
	// The resource node this unit is currently gathering
	protected ResourceNode gatherTarget;
	protected float gatherTimer = 0;
	
	protected Builder buildTarget;
	protected bool isBuilding = false;

	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
		
		if(gatherTarget != null) {
			UpdateGather();
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
				if(IsInGatherRange(gatherTarget.transform)) {
					gatherTarget.Gather(gatherAmount);
					playerStatus.GainResource(gatherTarget.resource, gatherAmount);
					gatherTimer = gatherTime;
				}
			}
		} else {
			// Not currently gathering
			if(IsInGatherRange(gatherTarget.transform)) {
				// In range, start gathering
				pathfinder.StopMoving();
				gatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				pathfinder.Move(gatherTarget.transform);
				gatherTimer = 0;
			}
		}
	}
	
	protected void UpdateBuild() {
		if(!isBuilding) {
			// Haven't started building yet
			if(IsInBuildRange(buildTarget.transform)) {
				pathfinder.StopMoving();
				isBuilding = true;
			} else {
				pathfinder.Move(buildTarget.transform);
			}
		} else {
			// Currently building
			buildTarget.Building(Time.deltaTime);
			if(buildTarget.IsComplete()) {
				StopBuilding();
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
		} else if(hit.collider.gameObject.CompareTag("Builder")) {
			attacker.StopAttacking();
			StopGathering();
			Build(hit.collider.gameObject.GetComponent<Builder>());
		}
	}
	
	// Sets this unit to gather from the given resource node
	public void Gather(ResourceNode node) {
		// If not already gathering this node
		if(node != gatherTarget) {
			gatherTimer = 0;
			gatherTarget = node;
		}
	}
	
	// Sets this unit to build the given creatable
	public void Build(Builder builder) {
		if(builder != buildTarget) {
			buildTarget = builder;
		}
	}
	
	public void StopGathering() {
		gatherTarget = null;
	}
	
	public void StopBuilding() {
		buildTarget = null;
	}
	
	public override void StopAllActions() {
		StopGathering();
		StopBuilding();
	}
	
	protected bool IsInGatherRange(Transform targetTransform) {
		return (targetTransform.position - this.transform.position).magnitude <= gatherRange;
	}
	
	protected bool IsInBuildRange(Transform targetTransform) {
		return (targetTransform.position - this.transform.position).magnitude <= buildRange;
	}
}
