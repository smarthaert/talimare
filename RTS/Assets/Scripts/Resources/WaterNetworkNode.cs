using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Node")]
public class WaterNetworkNode : MonoBehaviour {
	
	// The water network this node currently belongs to
	protected WaterNetwork Network { get; set; }
	
	public float supplyRange;
	protected Controllable controllable;
	
	protected SphereCollider SupplyCollider { get; set; }
	
	protected virtual void Awake() {
		controllable = GetComponent<Controllable>();
		
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		// A CapsuleCollider provides a trigger for the supply range
		SupplyCollider = child.AddComponent<SphereCollider>();
		SupplyCollider.isTrigger = true;
		SupplyCollider.radius = supplyRange;
	}
	
	protected virtual void Start() {
		// Evaluate objects already colliding
		foreach(Collider collider in Physics.OverlapSphere(SupplyCollider.transform.position, SupplyCollider.radius)) {
			OnTriggerEnter(collider);
		}
	}
	
	public void OnTriggerEnter(Collider other) {
		// Check to see if the other object is part of a WaterNetwork
		if(other.transform.parent.GetComponent<WaterNetworkNode>() != null && other.transform.parent.GetComponent<WaterNetworkNode>().Network != null) {
			if(Network == null) {
				// Join this network
				Network = other.transform.parent.GetComponent<WaterNetworkNode>().Network;
				Network.AddNode(this);
				this.transform.parent = Network.transform; //hierarchy not really needed, but useful for development
			} else {
				// Merge this network
				//TODO merge water networks
			}
		}
		if(Network != null && IsControllableWithSameOwner(other) && other.GetComponent<UnitStatus>() != null) {
			Network.AddSuppliable(other.GetComponent<UnitStatus>());
		}
	}
	
	public void OnTriggerExit(Collider other) {
		if(Network != null && IsControllableWithSameOwner(other) && other.GetComponent<UnitStatus>() != null) {
			Network.RemoveSuppliable(other.GetComponent<UnitStatus>());
		}
	}
	
	public bool IsControllableWithSameOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == controllable.Owner;
	}
	
	protected void OnDestroy() {
		if(Network != null) {
			Network.RemoveNode(this);
		}
	}
}