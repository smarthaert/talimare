using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Node")]
public class WaterNetworkNode : MonoBehaviour {
	
	public float supplyRange;
	
	// The water network this node currently belongs to
	public WaterNetwork Network { get; set; }
	
	// And its neighbors
	//TODO neighbors should be gathered on the fly. trying to keep track of them just isn't working out
	private HashSet<WaterNetworkNode> _neighbors = new HashSet<WaterNetworkNode>();
	public HashSet<WaterNetworkNode> Neighbors
	{
		get {
			_neighbors.RemoveWhere(m => m == null);
			return _neighbors;
		}
		protected set { _neighbors = value; }
	}
	
	// The objects which are currently within supply range and eligible for supply
	private List<UnitStatus> _suppliablesInRange = new List<UnitStatus>();
	public List<UnitStatus> SuppliablesInRange
	{
		get {
			_suppliablesInRange.RemoveAll(m => m == null);
			return _suppliablesInRange;
		}
		protected set { _suppliablesInRange = value; }
	}
	
	protected void Awake() {
		Neighbors = new HashSet<WaterNetworkNode>();
		SuppliablesInRange = new List<UnitStatus>();
		
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject("WaterNetworkNode");
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
	
	public virtual void NodeEnteredRange(WaterNetworkNode otherNode) {
		Debug.Log("I am: "+this+" and I am adding a neighbor: "+otherNode);
		Neighbors.Add(otherNode);
		// If other node is part of a network, join that network if we need one
		if(Network == null && otherNode.Network != null) {
			otherNode.Network.AddNode(this, true);
		}
	}
	
	public void NodeLeftRange(WaterNetworkNode otherNode) {
		Neighbors.Remove(otherNode);
	}
	
	public virtual void SuppliableEnteredRange(UnitStatus suppliable) {
		if(!SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
		}
	}
	
	public void SuppliableLeftRange(UnitStatus suppliable) {
		if(SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Remove(suppliable);
		}
	}
	
	protected void OnDestroy() {
		if(Network != null) {
			// Manually remove this node from the network to force the network to rebuild
			Network.RemoveNode(this, true);
		}
	}
}