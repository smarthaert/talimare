using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Source")]
public class WaterNetworkSource : WaterNetworkNode {
	
	public int waterSuppliedPerTick;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start() {
		base.Start();
		
		// If this source still has no WaterNetwork, create one
		if(Network == null) {
			GameObject waterNetwork = new GameObject("Water Network");
			waterNetwork.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
			this.transform.parent = waterNetwork.transform; //hierarchy not really needed, but useful for development
			Network = waterNetwork.AddComponent<WaterNetwork>();
		}
	}
}