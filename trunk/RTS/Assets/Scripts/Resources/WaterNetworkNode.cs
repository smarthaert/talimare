using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Node")]
public class WaterNetworkNode : MonoBehaviour {
	
	public float supplyRange;
	
	// The water network this node currently belongs to
	public WaterNetwork Network { get; set; }
	// And its neighbors
	public HashSet<WaterNetworkNode> Neighbors { get; protected set; }
	
	// The objects which are currently within supply range and eligible for supply
	protected HashSet<UnitStatus> SuppliablesInRange { get; set; }
	
	protected virtual void Awake() {
		Neighbors = new HashSet<WaterNetworkNode>();
		SuppliablesInRange = new HashSet<UnitStatus>();
		
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		
		// A SphereCollider provides a trigger for the supply range
		SphereCollider supplyCollider = child.AddComponent<SphereCollider>();
		supplyCollider.isTrigger = true;
		supplyCollider.radius = supplyRange;
		
		// A trigger script passes triggered events back to this one
		WaterNetworkNodeTrigger trigger = child.AddComponent<WaterNetworkNodeTrigger>();
		trigger.WaterNetworkNode = this;
		trigger.Controllable = GetComponent<Controllable>();
	}
	
	protected virtual void Start() {}
	
	public void NodeEnteredRange(WaterNetworkNode otherNode) {
		Neighbors.Add(otherNode);
		// If other node is part of a network, join that network if we need one
		if(Network == null && otherNode.Network != null) {
			otherNode.Network.AddNode(this);
		}
	}
	
	public void NodeLeftRange(WaterNetworkNode otherNode) {
		Neighbors.Remove(otherNode);
	}
	
	public void SuppliableEnteredRange(UnitStatus suppliable) {
		if(!SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
			if(Network != null) {
				Network.AddSuppliable(suppliable);
			}
		}
	}
	
	public void SuppliableLeftRange(UnitStatus suppliable) {
		if(SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Remove(suppliable);
			if(Network != null) {
				Network.RemoveSuppliable(suppliable);
			}
		}
	}
	
	public bool ContainsSuppliableInRange(UnitStatus suppliable) {
		return SuppliablesInRange.Contains(suppliable);
	}
	
	protected void OnDestroy() {
		if(Network != null) {
			Network.RemoveNode(this);
		}
	}
}