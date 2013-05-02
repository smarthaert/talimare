using UnityEngine;
using System.Collections.Generic;

// Keeps information about a unit's current status
public class UnitStatus : ControllableStatus {
	
	public int maxWater;
	// Unit loses this much water on each water tick
	public int waterLossRate;
	public int Water { get; protected set; }
	public float WaterPercentage { get { return Water / maxWater; } }
	// Holds all of the water suppliers of which this object is in range
	public List<WaterSupplier> WaterSuppliersInRange { get; protected set; }
	
	// Amount of time since last water tick
	protected float waterTickTimer = 0;
	// Used by a water supplier to mark that water should not be lost on the next water tick
	public bool CounteractWaterLoss { get; set; }
	
	protected override void Start() {
		base.Start();
		
		Water = maxWater;
		WaterSuppliersInRange = new List<WaterSupplier>();
		CounteractWaterLoss = false;
	}
	
	protected override void Update() {
		base.Update();
		
		if(waterLossRate != 0) {
			waterTickTimer += Time.deltaTime;
			if(waterTickTimer >= WaterSupplier.waterTickRate) {
				waterTickTimer = 0;
				if(CounteractWaterLoss) {
					CounteractWaterLoss = false;
				} else {
					Water -= waterLossRate;
				}
				if(Water <= 0) {
					Die();
				}
			}
		}
	}
	
	public void GainWater(int amount) {
		Water += amount;
		if(Water > maxWater)
			Debug.Log("Water is greater than max!");
		//shouldn't need to clamp water, as no water should ever be supplied above the maximum
		//Water = Mathf.Clamp(Water, 0, maxWater);
	}
	
	protected void OnDestroy() {
		foreach(WaterSupplier waterSupplier in WaterSuppliersInRange.ToArray()) {
			waterSupplier.OnTriggerExit(collider);
		}
	}
}
