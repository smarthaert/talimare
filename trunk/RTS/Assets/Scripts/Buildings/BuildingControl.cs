using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions
public class BuildingControl : OwnedObjectControl {
	
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
				if(unit.CanCreate(player)) {
					unit.SpendResources(player);
					unitQueue.Enqueue(unit);
				}
			}
		}
		foreach(Creatable tech in techs) {
			if(Input.GetKeyDown(tech.creationKey)) {
				if(!techQueue.Contains(tech) && tech.CanCreate(player)) {
					tech.SpendResources(player);
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
		newUnit.GetComponent<OwnedObjectControl>().player = player;
		if(rallyPoint != null) {
			newUnit.GetComponent<AIPathfinder>().Move(rallyPoint);
		}
		unitTimer = 0;
	}
	
	// Complete a tech, adding it to the player's tech list and running
	void CompleteTech() {
		Tech tech = techQueue.Dequeue().GetComponent<Tech>();
		tech.Execute(player);
	}
	
	// Called when another object moves into visual range
	public virtual void ObjectEnteredVision(GameObject obj) {
		
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		
	}
}