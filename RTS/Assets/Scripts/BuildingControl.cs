using UnityEngine;
using System.Collections.Generic;

// Parent class for all building controls. Contains general building utility functions
public abstract class BuildingControl : Selectable {
	
	// Used for keeping track of training in this building
	private Queue<TrainingTask> unitQueue = new Queue<TrainingTask>();
	private float unitTimer = 0;
	private Queue<TrainingTask> techQueue = new Queue<TrainingTask>();
	private float techTimer = 0;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if(unitQueue.Count > 0) {
			unitTimer += Time.deltaTime;
			if(unitTimer >= unitQueue.Peek().TrainingTime)
				CompleteUnit();
		}
		if(techQueue.Count > 0) {
			techTimer += Time.deltaTime;
			if(techTimer >= techQueue.Peek().TrainingTime)
				CompleteTech();
		}
	}
	
	protected void TrainUnit(TrainingTask trainingTask) {
		unitQueue.Enqueue(trainingTask);
	}
	
	protected void ResearchTech(TrainingTask trainingTask) {
		techQueue.Enqueue(trainingTask);
	}
	
	void CompleteUnit() {
		float distance = this.collider.bounds.size.magnitude + unitQueue.Peek().TrainingObject.collider.bounds.size.magnitude;
		Instantiate(unitQueue.Peek().TrainingObject, transform.position + (transform.right * distance), Quaternion.identity);
		unitQueue.Dequeue();
		unitTimer = 0;
	}
	
	void CompleteTech() {
		Instantiate(techQueue.Peek().TrainingObject, transform.position, Quaternion.identity);
		techQueue.Dequeue();
		techTimer = 0;
	}
}
