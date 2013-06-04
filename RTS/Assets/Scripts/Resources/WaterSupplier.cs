using UnityEngine;
using System.Collections.Generic;

public class WaterSupplier : LocalResourceSupplier {
	
	public int maxWaterHeld;
	public int waterGainRate;
	// The maximum amount of water per tick that can be supplied
	public int maxWaterSupplyRate = 25;
	public int WaterHeld { get; protected set; }
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 2;
	protected float waterTickTimer = 0;
	protected bool waterGained = false;
	
	// The objects which are currently within supply range and eligible for supply
	protected List<UnitStatus> suppliablesInRange = new List<UnitStatus>();
	
	protected override void Awake() {
		base.Awake();
		
		WaterHeld = 0;
	}
	
	protected override void Start() {
		base.Start();
	}
	
	protected void Update() {
		waterTickTimer += Time.deltaTime;
		if(waterTickTimer >= waterTickRate) {
			// Supply water
			waterTickTimer = 0;
			waterGained = false;
			SupplyWater();
		} else if(waterTickTimer >= waterTickRate/2 && !waterGained) {
			// Gain water
			waterGained = true;
			WaterHeld += waterGainRate;
			WaterHeld = Mathf.Clamp(WaterHeld, 0, maxWaterHeld);
		}
	}
	
	// An algorithm to supply water in an (in game terms) efficient manner to suppliables in range
	protected void SupplyWater() {
		if(suppliablesInRange.Count > 0) {
			int waterLeftToSupply = Mathf.Min(WaterHeld, maxWaterSupplyRate);
			int waterSupplied;
			// Remove any destroyed suppliables
			suppliablesInRange.RemoveAll(SuppliableIsDestroyed);
			// Sort suppliables from lowest to highest current water
			suppliablesInRange.Sort(CompareSuppliables);
			// First loop through suppliables, supplying only enough to cover their water loss rate
			foreach(UnitStatus suppliable in suppliablesInRange) {
				if(waterLeftToSupply > 0 && suppliable.waterLossRate > 0 && !suppliable.CounteractWaterLoss) {
					if(waterLeftToSupply >= suppliable.waterLossRate) {
						waterSupplied = suppliable.waterLossRate;
						suppliable.CounteractWaterLoss = true;
					} else {
						waterSupplied = Mathf.Min(waterLeftToSupply, suppliable.maxWater - suppliable.Water);
						if(waterSupplied > 0) {
							suppliable.GainWater(waterSupplied);
						}
					}
					waterLeftToSupply -= waterSupplied;
					WaterHeld -= waterSupplied;
					if(waterLeftToSupply == 0) {
						break;
					}
				}
			}
			// Second loop through suppliables, supplying the remainder of this tick's supply
			foreach(UnitStatus suppliable in suppliablesInRange) {
				if(waterLeftToSupply > 0 && suppliable.Water < suppliable.maxWater) {
					waterSupplied = Mathf.Min(waterLeftToSupply, suppliable.maxWater - suppliable.Water);
					suppliable.GainWater(waterSupplied);
					waterLeftToSupply -= waterSupplied;
					WaterHeld -= waterSupplied;
					if(waterLeftToSupply == 0) {
						break;
					}
				}
			}
		}
	}
	
	protected bool SuppliableIsDestroyed(UnitStatus s) {
		return !s.gameObject.activeInHierarchy;
	}
	
	protected int CompareSuppliables(UnitStatus x, UnitStatus y) {
		return x.WaterPercentage.CompareTo(y.WaterPercentage);
	}
	
	public override void OnTriggerEnter(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<UnitStatus>() != null && !suppliablesInRange.Contains(other.GetComponent<UnitStatus>())) {
			suppliablesInRange.Add(other.GetComponent<UnitStatus>());
			other.GetComponent<UnitStatus>().waterSuppliersInRange.Add(this);
		}
	}
	
	public override void OnTriggerExit(Collider other) {
		if(suppliablesInRange.Contains(other.GetComponent<UnitStatus>())) {
			suppliablesInRange.Remove(other.GetComponent<UnitStatus>());
			other.GetComponent<UnitStatus>().waterSuppliersInRange.Remove(this);
		}
	}
	
	protected void OnDestroy() {
		foreach(UnitStatus unitStatus in suppliablesInRange) {
			unitStatus.waterSuppliersInRange.Remove(this);
		}
	}
}