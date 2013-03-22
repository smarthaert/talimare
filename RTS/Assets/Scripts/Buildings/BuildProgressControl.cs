using UnityEngine;
using System;

// Handles the status of a building being placed and built
[RequireComponent(typeof(BuildingStatus))]
public class BuildProgressControl : Controllable {
	
	public Controllable finishedObject;
	
	protected static AstarPath pathfinding;
	[NonSerialized]
	public Creatable creatable;
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0;
	// Whether or not this building was completed (this is probably only available for one frame before this GameObject is destroyed)
	protected bool completed = false;
	
	protected override void Start() {
		base.Start();
		
		creatable = finishedObject.GetComponent<Creatable>();
		
		gameObject.GetComponent<BuildingStatus>().maxHP = finishedObject.GetComponent<BuildingStatus>().maxHP;
	}
	
	// Called when this building is committed (goes from a queued/placement state to actually being in the world)
	public void Commit() {
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		pathfinding.Scan();
		creatable.SpendResources(owner);
	}
	
	// Called at regular intervals while this building is being built to advance its completion
	public void Building(float timeSpent) {
		//TODO increase buildprogress's HP as it's built
		timeSpentCreating += timeSpent;
		Mathf.Clamp(timeSpentCreating, 0, creatable.creationTime);
		if(timeSpentCreating >= creatable.creationTime) {
			Complete();
		}
	}
	
	// Complete the building, instantiating its finished version, giving it a player, and destroying this progress object
	protected void Complete() {
		// If multiple civs are building this, there's a chance that Complete gets called more than once
		if(completed != true) {
			completed = true;
			Game.InstantiateControllable(finishedObject, owner, this.transform.position); //might also need to pass this.transform.rotation
			Destroy(this.gameObject);
		}
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / creatable.creationTime);
	}
	
	public bool Completed() {
		return completed;
	}
}

