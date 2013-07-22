using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Source")]
public class WaterNetworkSource : WaterNetworkNode {
	
	public int waterSuppliedPerTick;
	
	// First makes sure that this source has a network before calling the base function
	public override void NodeEnteredRange(WaterNetworkNode otherNode) {
		// If this source still has no WaterNetwork, create one around it
		if(Network == null) {
			createNetworkAroundSelf();
		}
		base.NodeEnteredRange(otherNode);
	}
	
	// First makes sure that this source has a network before calling the base function
	public override void SuppliableEnteredRange(UnitStatus suppliable) {
		// If this source still has no WaterNetwork, create one around it
		if(Network == null) {
			createNetworkAroundSelf();
		}
		base.SuppliableEnteredRange(suppliable);
	}
	
	public void createNetworkAroundSelf() {
		GameObject waterNetwork = new GameObject("Water Network");
		waterNetwork.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		waterNetwork.AddComponent<WaterNetwork>().AddNode(this, false);
	}
}