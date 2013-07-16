using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Source")]
public class WaterNetworkSource : WaterNetworkNode {
	
	public int maxWaterHeld;
	public int waterGainRate;
	// The maximum amount of water per tick that can be supplied
	public int maxWaterSupplyRate = 25;
	public int WaterHeld { get; protected set; }
	protected bool waterGained = false;
	
	protected override void Awake() {
		base.Awake();
		
		WaterHeld = 0;
	}
	
	protected override void Start() {
		base.Start();
	}
}