using UnityEngine;
using System.Collections.Generic;
using System;

// Contains general building utility functions
[AddComponentMenu("Buildings/Base Building Control")]
public class BaseBuildingControl : BuildingCommonControl {
	
	// Units this building can train
	public List<CreatableUnit> units;
	
	// Techs this building can research
	public List<CreatableTech> techs;
	
	// Used for keeping track of creation in this building
	protected Queue<UnitQueueEntry> unitQueue = new Queue<UnitQueueEntry>();
	protected CreateUnitJob currentUnitJob;
	protected Queue<CreatableTech> techQueue = new Queue<CreatableTech>();
	protected CreateTechJob currentTechJob;
	protected float techTimer = 0;
	
	protected Vector3? rallyPoint = null;
	
	protected override void Start() {
		base.Start();
		
		StoredResources = new Dictionary<Resource, int>();
		foreach(object resource in Enum.GetValues(typeof(Resource))) {
			StoredResources.Add((Resource)resource, 0);
		}
		
		GameUtil.RescanPathfinding();
	}
	
	protected void BuildControlMenus() {
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_UNITS, ControlStore.MENU_UNITS));
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_TECHS, ControlStore.MENU_TECHS));
		if(GetComponent<BuildingStatus>() != null && GetComponent<BuildingStatus>().powerRequired > 0) {
			ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.TOGGLE_POWER));
		}
		
		ControlMenu createUnitMenu = new ControlMenu();
		foreach(CreatableUnit unit in units) {
			createUnitMenu.MenuItems.Add(new ControlMenuItem(unit));
		}
		createUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		ControlMenus.Add(ControlStore.MENU_UNITS, createUnitMenu);
		
		ControlMenu createTechMenu = new ControlMenu();
		foreach(CreatableTech tech in techs) {
			createTechMenu.MenuItems.Add(new ControlMenuItem(tech, true));
		}
		createTechMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		ControlMenus.Add(ControlStore.MENU_TECHS, createTechMenu);
	}
	
	protected override void Update() {
		base.Update();
		
		// Advance unit creation
		if(currentUnitJob != null) {
			if(currentUnitJob.CreationStarted) {
				currentUnitJob.AdvanceCreationTime(Time.deltaTime);
				if(currentUnitJob.Completed) {
					CompleteUnitCreation();
				}
			} else if(currentUnitJob.ReadyForCreationStart) {
				BeginUnitCreation();
			}
		} else if(unitQueue.Count > 0) {
			StartCreateUnitJob();
		}
		
		// Advance tech creation
		if(currentTechJob != null) {
			if(currentTechJob.CreationStarted) {
				currentTechJob.AdvanceCreationTime(Time.deltaTime);
				if(currentTechJob.Completed) {
					CompleteTechCreation();
				}
			} else if(currentTechJob.ReadyForCreationStart) {
				BeginTechCreation();
			}
		} else if(techQueue.Count > 0) {
			StartCreateTechJob();
		}
	}
	
	public override void ReceiveMouseAction(RaycastHit hit) {
		base.ReceiveMouseAction(hit);
		
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			rallyPoint = hit.point;
		} else if(hit.transform.gameObject == this.gameObject) {
			rallyPoint = null;
		}
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.TOGGLE_POWER)) {
			GetComponent<BuildingStatus>().SetPowerEnabled(!GetComponent<BuildingStatus>().PowerEnabled);
		} else {
			// See if ControlCode exists in units or techs and if so, queue that Creatable
			foreach(CreatableUnit unit in units) {
				if(unit.ControlCode.Equals(controlCode) && unit.CanCreate(Owner).Bool) {
					//chooses a random unit to convert. only for development
					UnityEngine.Object obj = GameObject.FindObjectOfType(typeof(UnitStatus));
					if(obj != null) {
						QueueUnitToCreate(((UnitStatus)obj).GetComponent<Controllable>(), unit);
					} else {
						Debug.LogWarning("No units exist to convert!");
					}
				}
			}
			foreach(CreatableTech tech in techs) {
				if(tech.ControlCode.Equals(controlCode) && !techQueue.Contains(tech) && tech.CanCreate(Owner).Bool) {
					tech.SpendResources(Owner);
					techQueue.Enqueue(tech);
				}
			}
		}
	}
	
	public void QueueUnitToCreate(Controllable unitToConvert, CreatableUnit unitToCreate) {
		unitToCreate.SpendResources(Owner);
		unitQueue.Enqueue(new UnitQueueEntry(unitToConvert, unitToCreate));
	}
	
	protected void StartCreateUnitJob() {
		UnitQueueEntry dequeuedEntry = unitQueue.Dequeue();
		currentUnitJob = new CreateUnitJob(this, dequeuedEntry.UnitToConvert, dequeuedEntry.DestinationUnit, Owner, true);
		if(dequeuedEntry.UnitToConvert != null) {
			currentUnitJob.AssignNextJob(dequeuedEntry.UnitToConvert, false);
		}
	}
	
	// Begin a unit, making any converting unit disappear inside the building
	protected void BeginUnitCreation() {
		bool resourcesAllHere = true;
		foreach(ResourceAmount resourceAmount in currentUnitJob.DestinationUnit.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource() && StoredResources[resourceAmount.resource] < resourceAmount.amount) {
				resourcesAllHere = false;
			}
		}
		if(resourcesAllHere) {
			if(currentUnitJob.IsConversion) {
				currentUnitJob.Assignee.gameObject.SetActive(false);
			}
			foreach(ResourceAmount resourceAmount in currentUnitJob.DestinationUnit.resourceCosts) {
				if(!resourceAmount.IsUpkeepResource()) {
					StoredResources[resourceAmount.resource] -= resourceAmount.amount;
				}
			}
			currentUnitJob.CreationStarted = true;
		} else {
			Debug.LogError("Resources missing while beginning unit creation. Unit :"+currentUnitJob.DestinationUnit+", Building: "+this);
		}
	}
	
	// Complete a unit, instantiating it at a proper location, and giving it a rally point if necessary
	protected void CompleteUnitCreation() {
		GameObject newUnit;
		float distance = collider.bounds.size.magnitude + currentUnitJob.DestinationUnit.collider.bounds.size.magnitude;
		if(currentUnitJob.IsConversion) {
			newUnit = GameUtil.InstantiateConvertedControllable(currentUnitJob.Assignee, currentUnitJob.DestinationUnit.GetComponent<Controllable>(), Owner, transform.position + (transform.right * distance));
		} else {
			newUnit = GameUtil.InstantiateControllable(currentUnitJob.DestinationUnit.GetComponent<Controllable>(), Owner, transform.position + (transform.right * distance));
		}
		if(rallyPoint != null) {
			newUnit.GetComponent<Controllable>().AddTask(new MoveTask(newUnit.GetComponent<MoveTaskScript>(), rallyPoint), false);
		}
		currentUnitJob.RemoveAssignee(currentUnitJob.Assignee);
		currentUnitJob = null;
	}
	
	protected void StartCreateTechJob() {
		currentTechJob = new CreateTechJob(this, techQueue.Dequeue(), Owner, true);
	}
	
	// Begin a tech
	protected void BeginTechCreation() {
		bool resourcesAllHere = true;
		foreach(ResourceAmount resourceAmount in currentTechJob.Tech.resourceCosts) {
			if(!resourceAmount.IsUpkeepResource() && StoredResources[resourceAmount.resource] < resourceAmount.amount) {
				resourcesAllHere = false;
			}
		}
		if(resourcesAllHere) {
			foreach(ResourceAmount resourceAmount in currentTechJob.Tech.resourceCosts) {
				if(!resourceAmount.IsUpkeepResource()) {
					StoredResources[resourceAmount.resource] -= resourceAmount.amount;
				}
			}
			currentTechJob.CreationStarted = true;
		} else {
			Debug.LogError("Resources missing while beginning tech creation. Tech :"+currentTechJob.Tech+", Building: "+this);
		}
	}
	
	// Complete a tech
	protected void CompleteTechCreation() {
		currentTechJob.Tech.GetComponent<Tech>().AddTechForPlayer(Owner);
		currentTechJob = null;
	}
	
	protected void OnDestroy() {
		GameUtil.RescanPathfinding();
	}
}