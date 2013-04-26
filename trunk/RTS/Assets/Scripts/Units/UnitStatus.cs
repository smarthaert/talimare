using UnityEngine;
using System.Collections;

// Keeps information about a unit's current status
public class UnitStatus : ControllableStatus {
	
	public int maxWater;
	public int Water { get; protected set; }
	public int WaterLossRate { get; protected set; }
	
	protected void Start () {
		Water = maxWater;
		init();
	}
	
	private void init() {
		WaterLossRate = 1;
	}
	
	protected void Update () {
		//TODO: lose water / 5s
	}
}
