using UnityEngine;
using System.Collections.Generic;

public class WaterNetwork : MonoBehaviour {
	
	// All of the sources in this network
	private HashSet<WaterNetworkSource> _sources = new HashSet<WaterNetworkSource>();
	protected HashSet<WaterNetworkSource> Sources
	{
		get {
			_sources.RemoveWhere(m => m == null);
			return _sources;
		}
		set { _sources = value; }
	}
	
	// All of the nodes in this network, INCLUDING the sources
	private HashSet<WaterNetworkNode> _nodes = new HashSet<WaterNetworkNode>();
	protected HashSet<WaterNetworkNode> Nodes
	{
		get {
			_nodes.RemoveWhere(m => m == null);
			return _nodes;
		}
		set { _nodes = value; }
	}
	
	// The objects which are currently within supply range and eligible for supply
	private List<UnitStatus> _suppliablesInRange = new List<UnitStatus>();
	protected List<UnitStatus> SuppliablesInRange
	{
		get {
			_suppliablesInRange.RemoveAll(m => m == null);
			return _suppliablesInRange;
		}
		set { _suppliablesInRange = value; }
	}
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 2;
	protected float waterTickTimer = 0;
	
	protected void Awake() {
		Sources = new HashSet<WaterNetworkSource>();
		Nodes = new HashSet<WaterNetworkNode>();
		SuppliablesInRange = new List<UnitStatus>();
	}
	
	public void AddNode(WaterNetworkNode node) {
		if(node is WaterNetworkSource && !Sources.Contains((WaterNetworkSource)node)) {
			Sources.Add((WaterNetworkSource)node);
		} else if(!Nodes.Contains(node)) {
			Nodes.Add(node);
		}
		node.Network = this;
		node.transform.parent = this.transform; //hierarchy not really needed, but useful for development
		RebuildWaterNetwork(this);
	}
	
	public void RemoveNode(WaterNetworkNode node) {
		if(node is WaterNetworkSource) {
			Sources.Remove((WaterNetworkSource)node);
		} else {
			Nodes.Remove(node);
		}
		node.Network = null;
		node.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		RebuildWaterNetwork(this);
	}
	
	public void AddSuppliable(UnitStatus suppliable) {
		if(!SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
			Debug.Log("added suppliable to network");
		}
	}
	
	public void RemoveSuppliable(UnitStatus suppliable) {
		bool canRemove = true;
		foreach(WaterNetworkNode node in Nodes) {
			if(node.ContainsSuppliableInRange(suppliable)) {
				canRemove = false;
				break;
			}
		}
		if(canRemove) {
			SuppliablesInRange.Remove(suppliable);
			Debug.Log("removed suppliable from network");
		}
	}
	
	public static void RebuildWaterNetwork(WaterNetwork network) {
		Debug.Log("Rebuilding water network...");
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
			int waterLeftToSupply = 0;
			foreach(WaterNetworkSource source in Sources) {
				waterLeftToSupply += source.waterSuppliedPerTick;
			}
			// Sort suppliables from lowest to highest current water
			SuppliablesInRange.Sort(CompareSuppliables);
			// First loop through suppliables, supplying only enough to cover their water loss rate
			int waterSupplied;
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
	
	protected int CompareSuppliables(UnitStatus x, UnitStatus y) {
		return x.WaterPercentage.CompareTo(y.WaterPercentage);
	}
}