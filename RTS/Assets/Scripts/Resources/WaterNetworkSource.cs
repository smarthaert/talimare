using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Source")]
public class WaterNetworkSource : WaterNetworkNode {
	
	public int waterSuppliedPerTick;
	
	protected override void Start() {
		base.Start();
		
		// If this source still has no WaterNetwork, create one around it
		if(Network == null) {
			createNetworkAroundSelf(false);
		}
	}
	
	public void createNetworkAroundSelf(bool rebuildNetwork) {
		Debug.Log("No network, creating one if true: "+rebuildNetwork);
		GameObject waterNetwork = new GameObject("Water Network");
		waterNetwork.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		waterNetwork.AddComponent<WaterNetwork>().AddNode(this, rebuildNetwork);
	}
}