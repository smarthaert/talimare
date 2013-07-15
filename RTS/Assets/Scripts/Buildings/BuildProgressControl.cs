using UnityEngine;
using System.Collections.Generic;

// Handles the status of a building being placed and built
[AddComponentMenu("Buildings/Build Progress Control")]
public class BuildProgressControl : BuildingCommonControl {
	
	public BaseBuildingControl finishedBuilding;
	
	protected BuildingStatus BuildingStatus { get; set; }
	public CreatableBuilding FinishedBuildingCreatable { get; protected set; }
	// The global job associated with building this object
	public BuildJob BuildJob { get; protected set; }
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0f;
	// The rate at which object status updates while building
	protected static float statusUpdateRate = 0.5f;
	// The amount of time since the last status update
	protected float timeSinceLastUpdate = 0f;
	// Whether or not this building was completed (this is probably only available for one frame before this GameObject is destroyed)
	public bool Completed { get; protected set; }
	
	protected override void Awake() {
		base.Awake();
		
		BuildingStatus = gameObject.AddComponent<BuildingStatus>();
		gameObject.AddComponent<Vision>().visionRange = 2;
		FinishedBuildingCreatable = finishedBuilding.GetComponent<CreatableBuilding>();
	}
	
	protected override void Start() {
		base.Start();
		
		StoredResources = new Dictionary<Resource, int>();
		foreach(ResourceAmount resourceAmount in FinishedBuildingCreatable.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource()) {
				StoredResources.Add(resourceAmount.resource, 0);
			}
		}
	}
	
	protected override void BuildControlMenus() {
		ControlMenu baseBuildProgressMenu = new ControlMenu();
		baseBuildProgressMenu.MenuItems.Add(new ControlMenuItem(ControlStore.DESTROY));
		ControlMenus.Add(ControlStore.MENU_BASE, baseBuildProgressMenu);
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.DESTROY)) {
			Cancel();
		}
	}
	
	// Called when this building is committed (goes from a queued/placement state to actually being in the world)
	public void Commit() {
		Completed = false;
		BuildingStatus.maxHP = finishedBuilding.GetComponent<BuildingStatus>().maxHP;
		BuildingStatus.SetHP(1);
		GameUtil.RescanPathfinding();
		BuildJob = new BuildJob(this, Owner, true);
	}
	
	// Called at regular intervals while this building is being built to advance its completion
	public void Building(float timeSpent) {
		// Development error checking
		if(timeSpentCreating == 0f) {
			foreach(ResourceAmount resourceAmount in FinishedBuildingCreatable.resourceCosts) {
				if(!resourceAmount.IsUpkeepResource() && StoredResources[resourceAmount.resource] < resourceAmount.amount) {
					Debug.LogError(this+" is being built but is missing some of resource: "+resourceAmount.resource);
				}
			}
		}
		timeSpentCreating += timeSpent;
		if(timeSpentCreating >= FinishedBuildingCreatable.creationTime) {
			Complete();
		} else {
			timeSinceLastUpdate += timeSpent;
			if(timeSinceLastUpdate >= statusUpdateRate) {
				BuildingStatus.Heal((int)((timeSinceLastUpdate / FinishedBuildingCreatable.creationTime) * BuildingStatus.maxHP));
				timeSinceLastUpdate = 0f;
			}
		}
	}
	
	// Complete the building, instantiating its finished version and destroying this progress object
	protected void Complete() {
		// If multiple civs are building this, there's a chance that Complete gets called more than once
		if(!Completed) {
			Completed = true;
			GameObject newObject = GameUtil.InstantiateControllable(finishedBuilding, Owner, this.transform.position); //might also need to pass this.transform.rotation
			if(Game.PlayerInput.CurrentSelection == this) {
				Game.PlayerInput.Select(newObject.GetComponent<Selectable>());
			}
			Destroy(this.gameObject);
		}
	}
	
	// Cancel the building, destroying it
	protected void Cancel() {
		Destroy(this.gameObject);
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / FinishedBuildingCreatable.creationTime);
	}
}
