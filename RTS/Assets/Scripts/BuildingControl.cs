using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions
public class BuildingControl : Selectable {
	
	public List<Trainable> units;
	
	// Used for keeping track of training in this building
	private Queue<Trainable> unitQueue = new Queue<Trainable>();
	private float unitTimer = 0;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
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
	
	protected void TrainUnit(Trainable trainable) {
		unitQueue.Enqueue(trainable);
	}
	
	void CompleteUnit() {
		float distance = this.collider.bounds.size.magnitude + unitQueue.Peek().gameObject.collider.bounds.size.magnitude;
		Instantiate(unitQueue.Peek().gameObject, transform.position + (transform.right * distance), Quaternion.identity);
		unitQueue.Dequeue();
		unitTimer = 0;
	}
}
