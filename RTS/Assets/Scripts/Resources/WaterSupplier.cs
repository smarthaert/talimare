using UnityEngine;
using System.Collections.Generic;

public class WaterSupplier : MonoBehaviour {
	//TODO high: water supplier
	
	public int maxWaterHeld;
	public int waterGainRate;
	// The maximum amount of water per tick that can be supplied
	public int maxWaterSupplyRate = 25;
	public int WaterHeld { get; protected set; }
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 5;
	protected float waterTickTimer = 0;
	
	// The objects which are currently within supply range and eligible for supply
	protected List<UnitStatus> suppliablesInRange = new List<UnitStatus>();
	
	public float supplyRange;
	
	protected Controllable controllable;
	
	protected void Start() {
		// A capsule collider provides a trigger for the supply range
		CapsuleCollider supplyCollider = gameObject.AddComponent<CapsuleCollider>();
		supplyCollider.isTrigger = true;
		supplyCollider.radius = supplyRange;
		supplyCollider.height = 99f;
		
		// A rigidbody allows this object's collider to trigger while it is moving
		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;
		
		controllable = transform.parent.gameObject.GetComponent<Controllable>();
		
		WaterHeld = 0;
	}
	
	protected void Update() {
		waterTickTimer += Time.deltaTime;
		if(waterTickTimer >= waterTickRate) {
			// Supply water
			waterTickTimer = 0;
			SupplyWater();
		} else if(waterTickTimer >= waterTickRate/2) {
			// Gain water
			WaterHeld += waterGainRate;
			WaterHeld = Mathf.Clamp(WaterHeld, 0, maxWaterHeld);
		}
	}
	
	// An algorithm to supply water in an (in game terms) efficient manner to suppliables in range
	protected void SupplyWater() {
		if(suppliablesInRange.Count > 0) {
			int waterLeftToSupply = Mathf.Min(WaterHeld, maxWaterSupplyRate);
			int waterSupplied;
			int waterNeeded;
			// Sort suppliables from lowest to highest current water
			suppliablesInRange.Sort(CompareSuppliables);
			// First loop through suppliables, supplying only enough to cover their water loss rate
			foreach(UnitStatus suppliable in suppliablesInRange) {
				if(suppliable.Water == suppliable.maxWater) {
					waterNeeded = suppliable.waterLossRate;
				} else {
					waterNeeded = Mathf.Min(suppliable.maxWater - suppliable.Water, suppliable.waterLossRate);
				}
				if(waterNeeded > 0) {
					waterSupplied = Mathf.Min(waterLeftToSupply, waterNeeded);
					if(waterSupplied == suppliable.waterLossRate) {
						suppliable.CounteractWaterLoss = true;
					} else {
						suppliable.GainWater(waterSupplied);
					}
					waterLeftToSupply -= waterSupplied;
					if(waterLeftToSupply == 0) {
						break;
					}
				}
			}
			if(waterLeftToSupply > 0) {
				// Second loop through suppliables, supplying the remainder of this tick's supply
				foreach(UnitStatus suppliable in suppliablesInRange) {
					waterNeeded = suppliable.maxWater - suppliable.Water;
					if(waterNeeded > 0) {
						waterSupplied = Mathf.Min(waterLeftToSupply, waterNeeded);
						suppliable.GainWater(waterSupplied);
						waterLeftToSupply -= waterSupplied;
						if(waterLeftToSupply == 0) {
							break;
						}
					}
				}
			}
		}
	}
	
	protected int CompareSuppliables(UnitStatus x, UnitStatus y) {
		return x.Water.CompareTo(y.Water);
	}
	
	// Called when a collider enters this supply range
	protected void OnTriggerEnter(Collider other) {
		if(other.transform.parent != this.transform.parent) {
			if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner == controllable.owner) {
				// Object is a Controllable owned by this player
				if(other.GetComponent<UnitStatus>() != null) {
					Debug.Log(other.name + " entered supply range!");
					suppliablesInRange.Add(other.GetComponent<UnitStatus>());
				}
			}
		}
	}
	
	// Called when a collider exits this supply range
	protected void OnTriggerExit(Collider other) {
		if(other.transform.parent != this.transform.parent) {
			if(other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner == controllable.owner) {
				// Object is a Controllable owned by this player
				if(other.GetComponent<UnitStatus>() != null) {
					suppliablesInRange.Remove(other.GetComponent<UnitStatus>());
				}
			}
		}
	}
}