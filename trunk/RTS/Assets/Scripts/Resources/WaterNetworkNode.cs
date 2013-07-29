using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controllable))]
[AddComponentMenu("Resources/Water Network Node")]
public class WaterNetworkNode : MonoBehaviour {
	
	public float supplyRange;
	
	// The water network this node currently belongs to
	public WaterNetwork Network { get; set; }
	
	public Controllable Controllable { get; protected set; }
	protected SphereCollider SupplyCollider { get; set; }
	
	// The objects which are currently within supply range and eligible for supply
	private List<UnitStatus> _suppliablesInRange = new List<UnitStatus>();
	public List<UnitStatus> SuppliablesInRange
	{
		get {
			_suppliablesInRange.RemoveAll(m => m == null);
			return _suppliablesInRange;
		}
	}
	
	protected virtual void Awake() {
		Controllable = GetComponent<Controllable>();
		
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		
		// A SphereCollider provides a trigger for the supply range
		SupplyCollider = child.AddComponent<SphereCollider>();
		SupplyCollider.isTrigger = true;
		SupplyCollider.radius = supplyRange;
		
		// A trigger script passes triggered events back to this one
		WaterNetworkNodeTrigger trigger = child.AddComponent<WaterNetworkNodeTrigger>();
		trigger.WaterNetworkNode = this;
	}
	
	protected virtual void Start() {}
	
	public void NodeEnteredRange(WaterNetworkNode otherNode) {
		// If other node is part of a network, join that network if we need one
		if(Network == null && otherNode.Network != null) {
			otherNode.Network.AddNode(this, true);
		}
	}
	
	public void NodeLeftRange(WaterNetworkNode otherNode) {
		//nothing here yet
	}
	
	public void SuppliableEnteredRange(UnitStatus suppliable) {
		if(!SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
		}
	}
	
	public void SuppliableLeftRange(UnitStatus suppliable) {
		SuppliablesInRange.Remove(suppliable);
	}
	
	// Returns the neighbors of this node, gathered on the fly
	public ICollection<WaterNetworkNode> GetNeighbors() {
		HashSet<WaterNetworkNode> neighbors = new HashSet<WaterNetworkNode>();
		foreach(Collider other in Physics.OverlapSphere(SupplyCollider.transform.position, SupplyCollider.radius)) {
			if(other != SupplyCollider && other.GetComponent<WaterNetworkNodeTrigger>() != null &&
					other.transform.parent.GetComponent<Controllable>() != null &&
					other.transform.parent.GetComponent<Controllable>().Owner == Controllable.Owner) {
				neighbors.Add(other.GetComponent<WaterNetworkNodeTrigger>().WaterNetworkNode);
			}
		}
		return neighbors;
	}
	
	protected void OnDestroy() {
		if(Network != null) {
			// Manually remove this node from the network to force the network to rebuild
			Network.RemoveNode(this, true);
		}
	}
}