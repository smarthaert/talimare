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
	
	// Current upkeep resource maximums
	public int maxFood = 0;
	public int maxWater = 0;
	public int maxPower = 0;
	
	// Resources - use this when referencing resource levels from other scripts
	// Note: for upkeep resources, this contains the amount of usable resource remaining
	public Dictionary<Resource, int> resourceLevels = new Dictionary<Resource, int>();
	
	// Keeps track of all objects (units or buildings) which are using upkeep resources
	private Dictionary<Resource, Dictionary<UnityEngine.Object, int>> capturedUpkeepResources = new Dictionary<Resource, Dictionary<UnityEngine.Object, int>>();

	void Start () {
		food = maxFood;
		water = maxWater;
		power = maxPower;
		
		// Initialize the dictionaries by loading each with every resource
		foreach(object resource in Enum.GetValues(typeof(Resource))) {
			resourceLevels.Add((Resource)resource, (int)this.GetType().GetField(resource.ToString().ToLower()).GetValue(this));
			capturedUpkeepResources.Add((Resource)resource, new Dictionary<UnityEngine.Object, int>());
		}
		
		// Find all Trainables that currently exist and capture their upkeep resources
		foreach(Trainable trainable in FindObjectsOfType(typeof(Trainable)).Cast<Trainable>()) {
			foreach(ResourceAmount resourceCost in trainable.resourceCosts) {
				if(resourceCost.IsUpkeepResource()) {
					SpendResource(resourceCost.resource, resourceCost.amount);
					CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, trainable.gameObject);
				}
			}
		}
	}
	
	void Update () {
		
	}
	
	// Spends an amount of the given resource
	public void SpendResource(Resource resource, int amount) {
		resourceLevels[resource] -= amount;
	}
	
	// Captures an amount of an upkeep resource which is being used up by the given object (unit or building)
	public void CaptureUpkeepResource(Resource resource, int amount, UnityEngine.Object user) {
		capturedUpkeepResources[resource].Add(user, amount);
	}
}
