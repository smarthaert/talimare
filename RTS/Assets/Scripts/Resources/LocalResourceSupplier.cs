using UnityEngine;
using System.Collections.Generic;

public class LocalResourceSupplier : ResourceSupplier {
	
	//TODO high: local resource suppliers
	protected float supplyRange;
	
	protected Controllable controllable;
	
	protected void Start() {
		base.Start();
		
		// A capsule collider provides a trigger for the supply range
		CapsuleCollider supplyCollider = gameObject.AddComponent<CapsuleCollider>();
		supplyCollider.isTrigger = true;
		supplyCollider.radius = supplyRange;
		supplyCollider.height = 99f;
		
		// A rigidbody allows this object's collider to trigger while it is moving
		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;
		
		controllable = transform.parent.gameObject.GetComponent<Controllable>();
	}
	
	// Called when another collider enters this supply range
	protected void OnTriggerEnter(Collider other) {
		Debug.Log(other.name + " entered supply range!");
		if(other.transform.parent != this.transform.parent) {
			if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner != controllable.owner) {
				// Object is a Controllable owned by another player
				if(other.GetComponent<UnitStatus>() != null) {
					
				}
			}
		}
	}
	
	// Called when another collider exits this supply range
	protected void OnTriggerExit(Collider other) {
		if(other.transform.parent != this.transform.parent) {
			if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner != controllable.owner) {
				// Object is a Controllable owned by another player
				
			}
		}
	}
}