using UnityEngine;

// Catches triggered events and passes them to a parent WaterNetworkNode
public class WaterNetworkNodeTrigger : MonoBehaviour {
	
	public WaterNetworkNode WaterNetworkNode { get; set; }
	
	protected void Start() {
		// Evaluate objects already colliding
		/*foreach(Collider coll in Physics.OverlapSphere(collider.transform.position, ((SphereCollider)collider).radius)) {
			OnTriggerEnter(coll);
		}*/ //I don't think this is needed anymore? OnTriggerEnter seems to fire when the object is created anyway
	}
	
	public void OnTriggerEnter(Collider other) {
		//TODO when a new unit appears in range, this event doesn't fire
		if(other.GetComponent<WaterNetworkNodeTrigger>() != null && other.transform.parent.GetComponent<Controllable>() != null &&
				other.transform.parent.GetComponent<Controllable>().Owner == WaterNetworkNode.Controllable.Owner) {
			WaterNetworkNode.NodeEnteredRange(other.GetComponent<WaterNetworkNodeTrigger>().WaterNetworkNode);
		} else if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == WaterNetworkNode.Controllable.Owner && other.GetComponent<UnitStatus>() != null) {
			WaterNetworkNode.SuppliableEnteredRange(other.GetComponent<UnitStatus>());
		}
	}
	
	public void OnTriggerExit(Collider other) {
		if(other.GetComponent<WaterNetworkNodeTrigger>() != null && other.transform.parent.GetComponent<Controllable>() != null &&
				other.transform.parent.GetComponent<Controllable>().Owner == WaterNetworkNode.Controllable.Owner) {
			WaterNetworkNode.NodeLeftRange(other.GetComponent<WaterNetworkNodeTrigger>().WaterNetworkNode);
		} else if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == WaterNetworkNode.Controllable.Owner && other.GetComponent<UnitStatus>() != null) {
			WaterNetworkNode.SuppliableLeftRange(other.GetComponent<UnitStatus>());
		}
	}
}