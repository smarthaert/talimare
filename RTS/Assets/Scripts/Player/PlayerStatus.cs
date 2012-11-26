using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Keeps information about a player's current status
public class PlayerStatus : MonoBehaviour {
	
	// Resources - use these fields only for the initial setting of values in the editor
	[NonSerialized]
	public int food = 0;
	[NonSerialized]
	public int water = 0;
	[NonSerialized]
	public int power = 0;
	public int copper = 0;
	public int iron = 0;
	public int coal = 0;
	public int steel = 0;
	public int oil = 0;
	public int uranium = 0;
	public int unobtanium = 0;
	
	// Current upkeep resource maximums - use these fields only for the initial setting of values in the editor
	public int maxFood = 0;
	public int maxWater = 0;
	public int maxPower = 0;
	
	// Resources - use this when referencing resource levels from other scripts
	// Note: for upkeep resources, this contains the amount of usable resource remaining
	public Dictionary<Resource, int> resourceLevels = new Dictionary<Resource, int>();
	
	// Resource maximums - use this when referencing current upkeep resource maximums from other scripts
	public Dictionary<Resource, int> upkeepMaximums = new Dictionary<Resource, int>();
	
	// Keeps track of all objects (units or buildings) which are using upkeep resources
	private Dictionary<Resource, Dictionary<UnityEngine.Object, int>> capturedUpkeepResources = new Dictionary<Resource, Dictionary<UnityEngine.Object, int>>();
	
	// Techs the player has researched
	public HashSet<Tech> techs = new HashSet<Tech>();
	
	void Awake () {
		food = maxFood;
		water = maxWater;
		power = maxPower;
		
		upkeepMaximums.Add(Resource.Food, maxFood);
		upkeepMaximums.Add(Resource.Water, maxWater);
		upkeepMaximums.Add(Resource.Power, maxPower);
		
		// Initialize the dictionaries by loading each with every resource
		foreach(object resource in Enum.GetValues(typeof(Resource))) {
			resourceLevels.Add((Resource)resource, (int)this.GetType().GetField(resource.ToString().ToLower()).GetValue(this));
			capturedUpkeepResources.Add((Resource)resource, new Dictionary<UnityEngine.Object, int>());
		}
	}
	
	void Start () {
		// Find all Creatables that currently exist and spend their upkeep resources.
		// (This needs to be done since creatables that exist when the game starts were never queued,
		// and thus were never spent for)
		foreach(Creatable creatable in FindObjectsOfType(typeof(Creatable)).Cast<Creatable>()) {
			foreach(ResourceAmount resourceCost in creatable.resourceCosts) {
				if(resourceCost.IsUpkeepResource()) {
					SpendResource(resourceCost.resource, resourceCost.amount);
				}
			}
		}
	}
	
	void Update () {
		//later on, this will be checking if upkeep resources used have exceeded maximum limits
	}
	
	// Gains an amount of the given resource
	public void GainResource(Resource resource, int amount) {
		resourceLevels[resource] += amount;
		Debug.Log("Player gained "+amount+" "+resource);
	}
	
	// Spends an amount of the given resource. This is called when a unit, tech, or building is queued
	public void SpendResource(Resource resource, int amount) {
		resourceLevels[resource] -= amount;
	}
	
	// Adds a supplied amount of an upkeep resource that may be used
	public void AddSuppliedUpkeepResource(Resource resource, int amount) {
		upkeepMaximums[resource] += amount;
	}
	
	// Removes a supplied amount of an upkeep resource
	public void RemoveSuppliedUpkeepResource(Resource resource, int amount) {
		upkeepMaximums[resource] -= amount;
	}
	
	// Captures an amount of an upkeep resource which is being used up by the given object (unit or building).
	// This is called when a unit, tech, or building is actually instantiated
	public void CaptureUpkeepResource(Resource resource, int amount, UnityEngine.Object user) {
		capturedUpkeepResources[resource].Add(user, amount);
	}
	
	// Releases the given upkeep resource being used by the given object (unit or building).
	// This is called when the object is destroyed
	public void ReleaseUpkeepResource(Resource resource, UnityEngine.Object user) {
		capturedUpkeepResources[resource].Remove(user);
	}
}
