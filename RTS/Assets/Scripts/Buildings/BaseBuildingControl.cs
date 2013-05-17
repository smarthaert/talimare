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
	
	public const string UNIT_MENU_NAME = "unitMenu";
	public const string TECH_MENU_NAME = "techMenu";
	
	protected override void Start() {
		base.Start();
		
		GameUtil.RescanPathfinding();
	}
	
	protected override void BuildControlMenus() {
		ControlMenu baseBuildingMenu = new ControlMenu();
		baseBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_UNITS, UNIT_MENU_NAME));
		baseBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_TECHS, TECH_MENU_NAME));
		if(GetComponent<BuildingStatus>() != null && GetComponent<BuildingStatus>().powerRequired > 0) {
			baseBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.TOGGLE_POWER));
		}
		ControlMenus.Add(BASE_MENU_NAME, baseBuildingMenu);
		
		ControlMenu createUnitMenu = new ControlMenu();
		foreach(Creatable unit in units) {
			createUnitMenu.MenuItems.Add(new ControlMenuItem(unit));
		}
		createUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, BASE_MENU_NAME));
		ControlMenus.Add(UNIT_MENU_NAME, createUnitMenu);
		
		ControlMenu createTechMenu = new ControlMenu();
		foreach(Creatable tech in techs) {
			createTechMenu.MenuItems.Add(new ControlMenuItem(tech, true));
		}
		createTechMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, BASE_MENU_NAME));
		ControlMenus.Add(TECH_MENU_NAME, createTechMenu);
		
		CurrentControlMenu = ControlMenus[BASE_MENU_NAME];
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
		} else if(hit.collider.gameObject == this.gameObject) {
			rallyPoint = null;
		}
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.TOGGLE_POWER)) {
			GetComponent<BuildingStatus>().SetPowerEnabled(!GetComponent<BuildingStatus>().PowerEnabled);
		} else {
			// See if ControlCode exists in units or techs and if so, queue that Creatable
			foreach(Creatable unit in units) {
				if(unit.ControlCode.Equals(controlCode) && unit.CanCreate(Owner).Bool) {
					//TODO high: required resources need to be delivered to building
					unit.SpendResources(Owner);
					unitQueue.Enqueue(unit);
				}
			}
			foreach(Creatable tech in techs) {
				if(tech.ControlCode.Equals(controlCode) && !techQueue.Contains(tech) && tech.CanCreate(Owner).Bool) {
					tech.SpendResources(Owner);
					techQueue.Enqueue(tech);
				}
			}
		}
	}
	
	// Complete a unit, instantiating it at a proper location, and giving it a rally point if necessary
	protected void CompleteUnit() {
		Creatable unit = unitQueue.Dequeue();
		float distance = collider.bounds.size.magnitude + unit.gameObject.collider.bounds.size.magnitude;
		GameObject newUnit = GameUtil.InstantiateControllable(unit.GetComponent<Controllable>(), gameObject.GetComponent<Controllable>().Owner, transform.position + (transform.right * distance));
		if(rallyPoint != null) {
			newUnit.GetComponent<Controllable>().AddTask(new MoveTask(newUnit.GetComponent<MoveTaskScript>(), rallyPoint), false);
		}
		unitTimer = 0;
	}
	
	// Complete a tech, adding it to the player's tech list and running
	protected void CompleteTech() {
		Tech tech = techQueue.Dequeue().GetComponent<Tech>();
		tech.AddTechForPlayer(Owner);
		techTimer = 0;
	}
	
	protected void OnDestroy() {
		GameUtil.RescanPathfinding();
	}
}