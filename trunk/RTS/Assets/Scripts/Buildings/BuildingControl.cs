using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions
public class BuildingControl : Selectable {
	
	// Units this building can train
	public List<Trainable> units;
	
	// Used for keeping track of training in this building
	private Queue<Trainable> unitQueue = new Queue<Trainable>();
	private float unitTimer = 0;
	
	private PlayerStatus playerStatus;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent(typeof(PlayerStatus));
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if(unitQueue.Count > 0) {
			unitTimer += Time.deltaTime;
			if(unitTimer >= unitQueue.Peek().trainingTime)
				CompleteUnit();
		}
	}
	
	// Called when any key is pressed while this building is selected
	public override void KeyPressed() {
		// See if pressed key exists in units and if so, train that unit
		foreach(Trainable unit in units) {
			if(Input.GetKeyDown(unit.trainingKey)) {
				TrainUnit(unit);
			}
		}
	}
	
	// Queues a unit to train if all resource cost checks pass
	protected void TrainUnit(Trainable trainable) {
		bool canTrain = true;
		foreach(ResourceAmount resourceCost in trainable.resourceCosts) {
			if(playerStatus.resourceLevels[resourceCost.resource] < resourceCost.amount) {
				canTrain = false;
				Debug.Log ("Player does not have the required resource amount. Resource: "+resourceCost.resource+", Amount: "+resourceCost.amount);
				//show some nice error message to the player here
			}
		}
		if(canTrain) {
			foreach(ResourceAmount resourceCost in trainable.resourceCosts) {
				playerStatus.SpendResource(resourceCost.resource, resourceCost.amount);
			}
			unitQueue.Enqueue(trainable);
		}
	}
	
	// Complete a unit, instantiating it at a proper location
	void CompleteUnit() {
		Trainable unit = unitQueue.Dequeue();
		float distance = this.collider.bounds.size.magnitude + unit.gameObject.collider.bounds.size.magnitude;
		Object newObject = Instantiate(unit.gameObject, transform.position + (transform.right * distance), Quaternion.identity);
		// We can't actually capture a unit's upkeep resources until after it is instantiated, so do that here
		foreach(ResourceAmount resourceCost in unit.resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				playerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, newObject);
			}
		}
		unitTimer = 0;
	}
}
