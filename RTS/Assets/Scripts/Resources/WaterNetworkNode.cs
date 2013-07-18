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
	private List<UnitStatus> _suppliablesInRange = new List<UnitStatus>();
	protected List<UnitStatus> SuppliablesInRange
	{
		get {
			//GameUtil.ScrubNullsFromList<UnitStatus>(ref _suppliablesInRange);
			for(int i = _suppliablesInRange.Count - 1; i >= 0; i--) {
				Debug.Log("checking "+i+" which should be: "+_suppliablesInRange[i]);
				if(_suppliablesInRange[i] == null) {
					Debug.LogWarning("scrubbing nulls... coll:"+_suppliablesInRange.Count);
					_suppliablesInRange.RemoveAt(i);
					Debug.LogWarning("done scrubbing nulls... coll:"+_suppliablesInRange.Count);
				}
			}
			return _suppliablesInRange;
		}
		set { _suppliablesInRange = value; }
	}
	
	protected virtual void Awake() {
		Neighbors = new HashSet<WaterNetworkNode>();
		SuppliablesInRange = new List<UnitStatus>();
		
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
	
	protected void Update() {
		
		Debug.Log(SuppliablesInRange[0]);
	}
	
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
	
	protected bool ReferenceIsNull<T>(T reference) {
		if(reference == null)
			Debug.Log("null reference!");
		return reference == null;
	}
	
	protected void OnDestroy() {
		if(Network != null) {
			Network.RemoveNode(this);
		}
	}
}