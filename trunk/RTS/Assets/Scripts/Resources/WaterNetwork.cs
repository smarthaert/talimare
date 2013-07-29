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
	}
	
	// All of the nodes in this network, INCLUDING the sources
	private HashSet<WaterNetworkNode> _nodes = new HashSet<WaterNetworkNode>();
	protected HashSet<WaterNetworkNode> Nodes
	{
		get {
			_nodes.RemoveWhere(m => m == null);
			return _nodes;
		}
	}
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 2;
	protected float waterTickTimer = 0;
	
	// Adds a node to this network and optionally rebuilds the network
	public void AddNode(WaterNetworkNode node, bool rebuildNetwork) {
		if(node is WaterNetworkSource && !Sources.Contains((WaterNetworkSource)node)) {
			Sources.Add((WaterNetworkSource)node);
		}
		if(!Nodes.Contains(node)) {
			Nodes.Add(node);
		}
		
		node.Network = this;
		node.transform.parent = this.transform; //hierarchy not really needed, but useful for development
		
		if(rebuildNetwork) {
			RebuildWaterNetwork(this);
		}
	}
	
	// Removes a node from this network and optionally rebuilds the network
	// Currently, this is never called. The only ways to be removed from a network are to rebuild it or be destroyed.
	public void RemoveNode(WaterNetworkNode node, bool rebuildNetwork) {
		if(node is WaterNetworkSource) {
			Sources.Remove((WaterNetworkSource)node);
		}
		Nodes.Remove(node);
		
		node.Network = null;
		node.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		
		if(rebuildNetwork) {
			RebuildWaterNetwork(this);
		}
	}
	
	// Rebuilds the water network, flowing outward from each source. Any disconnected sources will have new networks created,
	// while any disconnected non-source nodes will end with no network
	protected void RebuildWaterNetwork(WaterNetwork network) {
		HashSet<WaterNetworkNode> closedSet = new HashSet<WaterNetworkNode>();
		//reset all nodes' networks for the scenario where a node has been removed
		foreach(WaterNetworkNode node in Nodes) {
			Destroy(node.Network.gameObject); //this line may be causing some errors when stopping the game in development
			node.Network = null;
			node.transform.parent = this.transform.parent; //hierarchy not really needed, but useful for development
		}
		//start at each source
		foreach(WaterNetworkSource source in Sources) {
			if(!closedSet.Contains(source)) {
				//if the source hasn't already been accounted for, create a network around it
				source.createNetworkAroundSelf();
				//then start flowing outward recursively through its neighbors
				foreach(WaterNetworkNode neighbor in source.GetNeighbors()) {
					AddNodeToNetwork(source.Network, neighbor, closedSet);
				}
			}
		}
	}
	
	// Adds the node and all of its neighbors to the given network
	protected void AddNodeToNetwork(WaterNetwork network, WaterNetworkNode node, HashSet<WaterNetworkNode> closedSet) {
		if(!closedSet.Contains(node)) {
			network.AddNode(node, false);
			closedSet.Add(node);
			foreach(WaterNetworkNode neighbor in node.GetNeighbors()) {
				AddNodeToNetwork(network, neighbor, closedSet);
			}
		}
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
		//TODO test water supply with mutliple units
		List<UnitStatus> suppliablesInRange = new List<UnitStatus>();
		foreach(WaterNetworkNode node in Nodes) {
			suppliablesInRange.AddRange(node.SuppliablesInRange);
		}
		if(suppliablesInRange.Count > 0) {
			int waterLeftToSupply = 0;
			foreach(WaterNetworkSource source in Sources) {
				waterLeftToSupply += source.waterSuppliedPerTick;
			}
			// Sort suppliables from lowest to highest current water
			suppliablesInRange.Sort(CompareSuppliables);
			// First loop through suppliables, supplying only enough to cover their water loss rate
			int waterSupplied;
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