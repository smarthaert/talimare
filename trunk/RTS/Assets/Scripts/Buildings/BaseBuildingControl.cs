using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions
public class BaseBuildingControl : Controllable {
	
	// Units this building can train
	public List<Creatable> units;
	
	// Techs this building can research
	public List<Creatable> techs;
	
	// Used for keeping track of creation in this building
	protected Queue<Creatable> unitQueue = new Queue<Creatable>();
	protected float unitTimer = 0;
	protected Queue<Creatable> techQueue = new Queue<Creatable>();
	protected float techTimer = 0;
	
	protected Vector3? rallyPoint = null;
	
	protected override void Start() {
		base.Start();
	}
	
	protected override void BuildControlMenus() {
		ControlMenu baseBuildingMenu = new ControlMenu("baseBuilding");
		baseBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_UNITS, "createUnit"));
		baseBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_TECHS, "createTech"));
		ControlMenus.Add(baseBuildingMenu);
		
		ControlMenu createUnitMenu = new ControlMenu("createUnit");
		foreach(Creatable unit in units) {
			createUnitMenu.MenuItems.Add(new ControlMenuItem(unit, null));
		}
		createUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, "baseBuilding"));
		ControlMenus.Add(createUnitMenu);
		
		ControlMenu createTechMenu = new ControlMenu("createTech");
		foreach(Creatable tech in techs) {
			createTechMenu.MenuItems.Add(new ControlMenuItem(tech, null));
		}
		createTechMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, "baseBuilding"));
		ControlMenus.Add(createTechMenu);
		
		CurrentControlMenu = ControlMenus[0];
	}
	
	protected override void Update() {
		base.Update();
		
		// Advance creation queues
		if(unitQueue.Count > 0) {
			unitTimer += Time.deltaTime;
			if(unitTimer >= unitQueue.Peek().creationTime)
				CompleteUnit();
		}
		if(techQueue.Count > 0) {
			techTimer += Time.deltaTime;
			if(techTimer >= techQueue.Peek().creationTime)
				CompleteTech();
		}
	}
	
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			rallyPoint = hit.point;
			Debug.Log("Rally point set to: "+rallyPoint);
		} else if(hit.collider.gameObject == this.gameObject) {
			rallyPoint = null;
			Debug.Log("Rally point removed.");
		}
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		// See if ControlCode exists in units or techs and if so, queue that Creatable
		foreach(Creatable unit in units) {
			if(unit.ControlCode.Equals(controlCode) && unit.CanCreate(owner).Bool) {
				unit.SpendResources(owner);
				unitQueue.Enqueue(unit);
			}
		}
		foreach(Creatable tech in techs) {
			if(tech.ControlCode.Equals(controlCode) && !techQueue.Contains(tech) && tech.CanCreate(owner).Bool) {
				tech.SpendResources(owner);
				techQueue.Enqueue(tech);
			}
		}
	}
	
	// Complete a unit, instantiating it at a proper location, and giving it a rally point if necessary
	void CompleteUnit() {
		Creatable unit = unitQueue.Dequeue();
		float distance = collider.bounds.size.magnitude + unit.gameObject.collider.bounds.size.magnitude;
		GameObject newUnit = GameUtil.InstantiateControllable(unit.GetComponent<Controllable>(), gameObject.GetComponent<Controllable>().owner, transform.position + (transform.right * distance));
		if(rallyPoint != null) {
			newUnit.GetComponent<Controllable>().AddTask(new Task(newUnit.GetComponent<MoveTaskScript>(), rallyPoint), false);
		}
		unitTimer = 0;
	}
	
	// Complete a tech, adding it to the player's tech list and running
	void CompleteTech() {
		Tech tech = techQueue.Dequeue().GetComponent<Tech>();
		tech.AddTechForPlayer(owner);
		techTimer = 0;
	}
}