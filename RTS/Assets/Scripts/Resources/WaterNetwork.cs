using UnityEngine;
using System.Collections.Generic;

public class WaterNetwork : MonoBehaviour {
	
	protected List<WaterNetworkSource> Sources { get; set; }
	protected List<WaterNetworkNode> Nodes { get; set; }
	
	// The objects which are currently within supply range and eligible for supply
	protected List<UnitStatus> SuppliablesInRange { get; set; }
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 2;
	protected float waterTickTimer = 0;
	
	protected void Awake() {
		Sources = new List<WaterNetworkSource>();
		Nodes = new List<WaterNetworkNode>();
		SuppliablesInRange = new List<UnitStatus>();
	}
	
	public void AddNode(WaterNetworkNode node) {
		if(node is WaterNetworkSource) {
			Sources.Add((WaterNetworkSource)node);
		} else {
			Nodes.Add(node);
		}
	}
	
	public void RemoveNode(WaterNetworkNode node) {
		if(node is WaterNetworkSource) {
			Sources.Remove((WaterNetworkSource)node);
		} else {
			Nodes.Remove(node);
		}
	}
	
	public void AddSuppliable(UnitStatus suppliable) {
		Debug.Log("add suppliable");
		if(!SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
			suppliable.WaterNetwork = this;
		}
	}
	
	public void RemoveSuppliable(UnitStatus suppliable) {
		Debug.Log("remove suppliable");
		SuppliablesInRange.Remove(suppliable);//TODO only remove if no other nodes or sources contain this suppliable
		suppliable.WaterNetwork = null;
	}
	
	protected void Update() {
		waterTickTimer += Time.deltaTime;
		if(waterTickTimer >= waterTickRate) {
			waterTickTimer = 0;
			SupplyWater();
		}
	}
	
	// An algorithm to supply water in an (in game terms) efficient manner to suppliables in range
	protected void SupplyWater() {
		if(SuppliablesInRange.Count > 0) {
			foreach(WaterNetworkSource source in Sources) {
				int waterLeftToSupply = source.waterSuppliedPerTick;
				int waterSupplied;
				// Remove any destroyed suppliables - this shouldn't be necessary anymore?
				//SuppliablesInRange.RemoveAll(SuppliableIsDestroyed);
				// Sort suppliables from lowest to highest current water
				SuppliablesInRange.Sort(CompareSuppliables);
				// First loop through suppliables, supplying only enough to cover their water loss rate
				foreach(UnitStatus suppliable in SuppliablesInRange) {
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
						if(waterLeftToSupply == 0) {
							break;
						}
					}
				}
				// Second loop through suppliables, supplying the remainder of this tick's supply
				foreach(UnitStatus suppliable in SuppliablesInRange) {
					if(waterLeftToSupply > 0 && suppliable.Water < suppliable.maxWater) {
						waterSupplied = Mathf.Min(waterLeftToSupply, suppliable.maxWater - suppliable.Water);
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
	
	protected bool SuppliableIsDestroyed(UnitStatus s) {
		return !s.gameObject.activeInHierarchy;
	}
	
	protected int CompareSuppliables(UnitStatus x, UnitStatus y) {
		return x.WaterPercentage.CompareTo(y.WaterPercentage);
	}
}