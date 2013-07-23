using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Source")]
public class WaterNetworkSource : WaterNetworkNode {
	
	public int waterSuppliedPerTick;
	
	protected override void Start() {
		base.Start();
		
		// If this source isn't already touching a node that belongs to a network, create a new network around it.
		// Otherwise, NodeEnteredRange will take care of joining the network later
		bool createNetwork = true;
		foreach(Collider other in Physics.OverlapSphere(SupplyCollider.transform.position, SupplyCollider.radius)) {
			if(other != SupplyCollider && other.GetComponent<WaterNetworkNodeTrigger>() != null &&
					other.transform.parent.GetComponent<Controllable>() != null &&
					other.transform.parent.GetComponent<Controllable>().Owner == Controllable.Owner &&
					other.GetComponent<WaterNetworkNodeTrigger>().WaterNetworkNode.Network != null) {
				createNetwork = false;
				break;
			}
		}
		if(createNetwork) {
			createNetworkAroundSelf();
		}
	}
	
	// Creates a new water network and adds this source to it
	public void createNetworkAroundSelf() {
		GameObject waterNetwork = new GameObject("Water Network");
		waterNetwork.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		waterNetwork.AddComponent<WaterNetwork>().AddNode(this, false);
	}
}