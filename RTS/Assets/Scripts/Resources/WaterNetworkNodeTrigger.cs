using UnityEngine;

// Catches triggered events and passes them to a parent WaterNetworkNode
public class WaterNetworkNodeTrigger : MonoBehaviour {
	
	public WaterNetworkNode WaterNetworkNode { get; set; }
	public Controllable Controllable { get; set; }
	
	protected void Start() {
		// Evaluate objects already colliding
		foreach(Collider coll in Physics.OverlapSphere(collider.transform.position, ((SphereCollider)collider).radius)) {
			OnTriggerEnter(coll);
		}
	}
	
	public void OnTriggerEnter(Collider other) {
		//TODO when a new unit appears in range, this event fires but the unit has no Controllable component
		if(other.name.Equals("WaterNetworkNode")) {
			WaterNetworkNode.NodeEnteredRange(other.transform.parent.GetComponent<WaterNetworkNode>());
		} else if(IsSuppliableWithSameOwner(other)) {
			WaterNetworkNode.SuppliableEnteredRange(other.GetComponent<UnitStatus>());
		}
	}
	
	public void OnTriggerExit(Collider other) {
		if(other.name.Equals("WaterNetworkNode")) {
			WaterNetworkNode.NodeLeftRange(other.transform.parent.GetComponent<WaterNetworkNode>());
		} else if(IsSuppliableWithSameOwner(other)) {
			WaterNetworkNode.SuppliableLeftRange(other.GetComponent<UnitStatus>());
		}
	}
	
	protected bool IsSuppliableWithSameOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == Controllable.Owner && other.GetComponent<UnitStatus>() != null;
	}
}