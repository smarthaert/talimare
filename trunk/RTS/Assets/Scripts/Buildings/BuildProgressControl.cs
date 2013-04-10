using UnityEngine;
using System;

// Handles the status of a building being placed and built
[RequireComponent(typeof(BuildingStatus))]
public class BuildProgressControl : Controllable {
	
	public Controllable finishedObject;
	
	protected static AstarPath pathfinding;
	
	protected BuildingStatus BuildingStatus { get; set; }
	public Creatable Creatable { get; set; }
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0f;
	// The rate at which object status updates while building
	protected static float statusUpdateRate = 0.5f;
	// The amount of time since the last status update
	protected float timeSinceLastUpdate = 0f;
	// Whether or not this building was completed (this is probably only available for one frame before this GameObject is destroyed)
	protected bool completed = false;
	
	protected override void Start() {
		base.Start();
		
		BuildingStatus = gameObject.GetComponent<BuildingStatus>();
		Creatable = finishedObject.GetComponent<Creatable>();
	}
	
	// Called when this building is committed (goes from a queued/placement state to actually being in the world)
	public void Commit() {
		BuildingStatus.maxHP = finishedObject.GetComponent<BuildingStatus>().maxHP;
		BuildingStatus.SetHPToZero();
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		pathfinding.Scan();
		Creatable.SpendResources(owner);
	}
	
	// Called at regular intervals while this building is being built to advance its completion
	public void Building(float timeSpent) {
		timeSpentCreating += timeSpent;
		if(timeSpentCreating >= Creatable.creationTime) {
			Complete();
		} else {
			timeSinceLastUpdate += timeSpent;
			if(timeSinceLastUpdate >= statusUpdateRate) {
				BuildingStatus.Heal((int)((timeSinceLastUpdate / Creatable.creationTime) * BuildingStatus.maxHP));
				timeSinceLastUpdate = 0f;
			}
		}
	}
	
	// Complete the building, instantiating its finished version, giving it a player, and destroying this progress object
	protected void Complete() {
		// If multiple civs are building this, there's a chance that Complete gets called more than once
		if(completed != true) {
			completed = true;
			GameUtil.InstantiateControllable(finishedObject, owner, this.transform.position); //might also need to pass this.transform.rotation
			Destroy(this.gameObject);
		}
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / Creatable.creationTime);
	}
	
	public bool Completed() {
		return completed;
	}
}

