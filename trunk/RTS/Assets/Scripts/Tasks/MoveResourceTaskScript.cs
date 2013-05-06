using UnityEngine;
using System.Collections.Generic;

// Handles moving of resources
[RequireComponent(typeof(MoveTaskScript))]
public class MoveResourceTaskScript : MonoBehaviour {
	
	protected MoveResourceJob MoveResourceJob { get; set; }
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	protected void Update () {
		if(MoveResourceJob != null) {
			UpdateMoveResource();
		}
	}
	
	protected void UpdateMoveResource() {
		//TODO move resource - check if the job has been fulfilled yet and if not, check if we are holding any resources to move and if not, fetch some
	}
	
	public void StartTask(MoveResourceJob moveResourceJob) {
		if(MoveResourceJob != moveResourceJob) {
			MoveResourceJob = moveResourceJob;
		}
	}
	
	public bool IsTaskRunning() {
		return MoveResourceJob != null;
	}
	
	public void StopTask() {
		MoveResourceJob = null;
	}
	
	protected bool IsInRange(GameObject gameObject) {
		float range = this.collider.bounds.size.magnitude/2 + gameObject.collider.bounds.size.magnitude/2 + 0.5f;
		return (gameObject.transform.position - this.transform.position).magnitude <= range;
	}
}
