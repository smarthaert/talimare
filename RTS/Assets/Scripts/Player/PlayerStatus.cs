using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Keeps information about a player's current status
[AddComponentMenu("Player/Player Status")]
public class PlayerStatus : MonoBehaviour {
	
	// Current upkeep resource maximums - use these fields only for the initial setting of values in the editor
	public int maxFood = 0;
	
	// Resources - use this when referencing resource levels from other scripts
	// Note: for upkeep resources, this contains the amount of usable resource remaining
	public Dictionary<Resource, int> resourceLevels = new Dictionary<Resource, int>();
	
	// Resource maximums - use this when referencing current upkeep resource maximums from other scripts
	public Dictionary<Resource, int> upkeepMaximums = new Dictionary<Resource, int>();
	
	// Keeps track of all objects (units or buildings) which are using upkeep resources
	protected Dictionary<Resource, Dictionary<UnityEngine.Object, int>> capturedUpkeepResources = new Dictionary<Resource, Dictionary<UnityEngine.Object, int>>();
	
	// Techs the player has researched
	public HashSet<Tech> techs = new HashSet<Tech>();
	
	protected Player player;
	
	protected void Awake() {
		player = GetComponent<Player>();
		
		upkeepMaximums.Add(Resource.Food, maxFood);
		
		// Initialize the dictionaries by loading each with every resource
		foreach(object resource in Enum.GetValues(typeof(Resource))) {
			resourceLevels.Add((Resource)resource, 0);
			capturedUpkeepResources.Add((Resource)resource, new Dictionary<UnityEngine.Object, int>());
		}
	}
	
	protected void Start() {
		// Find all Creatables that currently exist and spend their resources.
		// (This needs to be done since Creatables that exist when the game starts were never queued,
		// and thus were never spent for)
		foreach(Creatable creatable in FindObjectsOfType(typeof(Creatable)).Cast<Creatable>()) {
			if(creatable.gameObject.GetComponent<Controllable>().Owner == player) {
				foreach(ResourceAmount resourceCost in creatable.resourceCosts) {
					if(resourceCost.IsUpkeepResource()) { //only really need to do upkeep resources
						LoseResource(resourceCost.resource, resourceCost.amount);
					}
				}
			}
		}
	}
	
	// Gains an amount of the given resource
	public void GainResource(Resource resource, int amount) {
		resourceLevels[resource] += amount;
	}
	
	// Spends an amount of the given resource. This is called when a unit, tech, or building is queued
	public void LoseResource(Resource resource, int amount) {
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
