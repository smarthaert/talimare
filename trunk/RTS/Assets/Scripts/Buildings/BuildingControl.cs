using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions
public class BuildingControl : SelectableControl {
	
	// Units this building can train
	public List<Creatable> units;
	
	// Techs this building can research
	public List<Creatable> techs;
	
	// Used for keeping track of creation in this building
	private Queue<Creatable> unitQueue = new Queue<Creatable>();
	private float unitTimer = 0;
	private Queue<Creatable> techQueue = new Queue<Creatable>();
	private float techTimer = 0;
	
	private static PlayerStatus playerStatus;
	
	void Awake () {
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent(typeof(PlayerStatus));
	}
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
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
	
	// Called when any key is pressed while this building is selected
	public override void KeyPressed() {
		// See if pressed key exists in units and if so, train that unit
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
				if(tech.CanCreate()) {
					tech.SpendResources();
					techQueue.Enqueue(tech);
				}
			}
		}
	}
	
	// Complete a unit, instantiating it at a proper location
	void CompleteUnit() {
		Creatable unit = unitQueue.Dequeue();
		float distance = this.collider.bounds.size.magnitude + unit.gameObject.collider.bounds.size.magnitude;
		Instantiate(unit.gameObject, transform.position + (transform.right * distance), Quaternion.identity);
		unitTimer = 0;
	}
	
	// Complete a tech, adding it to the player's tech list and running
	void CompleteTech() {
		Tech tech = (Tech)techQueue.Dequeue().GetComponent(typeof(Tech));
		playerStatus.techs.Add(tech);
		tech.Execute();
	}
}