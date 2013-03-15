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
		
		//TODO set the owner of all units and techs and buildings to this owner
		
		units = units.ConvertAll<Creatable>(MakePrefabRuntimeCopy); //TODO !! this doesn't work with multiple starting buildings (each building cloned from the same prefab needs to reference the same unit templates)
																	//for every creatable: is there a template for me yet? if so, replace my reference to that one. if not, create a template for me
	}
	
	// Used to create a copy of a prefab at runtime so that we can modify its values without modifying the underlying prefab asset.
	// (For units, this copy is called the Unit Template. This Template is cloned to create individual units, so when a unit needs
	// to be changed [upgraded, etc.] the Template can be changed and all new instances of that unit will receive the changes. Note
	// that existing units which were cloned from the Template will not be automatically updated.)
	protected Creatable MakePrefabRuntimeCopy(Creatable creatable) {
		GameObject copy = (GameObject)Instantiate(creatable.gameObject);
		copy.name = creatable.gameObject.name;
		copy.GetComponent<Controllable>().owner = owner;
		copy.SetActive(false);
		return copy.GetComponent<Creatable>();
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
	
	// Called when mouse action button is clicked on any object while this building is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			rallyPoint = hit.point;
			Debug.Log("Rally point set to: "+rallyPoint);
		} else if(hit.collider.gameObject == this.gameObject) {
			rallyPoint = null;
			Debug.Log("Rally point removed.");
		}
	}
	
	// Called when any key is pressed while this building is selected
	public override void KeyPressed() {
		// See if pressed key exists in units or techs and if so, queue that Creatable
		foreach(Creatable unit in units) {
			if(Input.GetKeyDown(unit.creationKey)) {
				if(unit.CanCreate()) {
					unit.SpendResources();
					unitQueue.Enqueue(unit);
				}
			}
		}
		foreach(Creatable tech in techs) {
			if(Input.GetKeyDown(tech.creationKey)) {
				if(!techQueue.Contains(tech) && tech.CanCreate()) {
					tech.SpendResources();
					techQueue.Enqueue(tech);
				}
			}
		}
	}
	
	// Complete a unit, instantiating it at a proper location, assigning it a Player, and giving it a rally point if necessary
	void CompleteUnit() {
		Creatable unit = unitQueue.Dequeue();
		float distance = this.collider.bounds.size.magnitude + unit.gameObject.collider.bounds.size.magnitude;
		GameObject newUnit = (GameObject)Instantiate(unit.gameObject, transform.position + (transform.right * distance), Quaternion.identity);
		newUnit.name = unit.gameObject.name;
		unit.GetComponent<UnitStatus>().maxHP = unit.GetComponent<UnitStatus>().maxHP + 2;
		newUnit.SetActive(true);
		if(rallyPoint != null) {
			newUnit.GetComponent<AIPathfinder>().Move(rallyPoint);
		}
		unitTimer = 0;
	}
	
	// Complete a tech, adding it to the player's tech list and running
	void CompleteTech() {
		Tech tech = techQueue.Dequeue().GetComponent<Tech>();
		tech.Execute(owner);
	}
}