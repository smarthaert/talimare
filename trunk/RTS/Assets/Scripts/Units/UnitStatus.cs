using UnityEngine;
using System.Collections;

// Keeps information about a unit's current status
public class UnitStatus : ControllableStatus {
	
	public int maxWater;
	// Unit loses this much water on each water tick
	public int waterLossRate;
	public int Water { get; protected set; }
	// Used by a water supplier to mark that water should not be lost on the next water tick
	public bool CounteractWaterLoss { get; set; }
	
	// Amount of time since last water tick
	protected float waterTickTimer = 0;
	
	protected override void Start() {
		base.Start();
		
		Water = maxWater;
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
		Mathf.Clamp(Water, 0, maxWater);
	}
}
