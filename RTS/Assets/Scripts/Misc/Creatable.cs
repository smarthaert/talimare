using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, building, or tech)
public class Creatable : MonoBehaviour {
	
	public float creationTime;
	public List<ResourceAmount> resourceCosts;
	// Holds the resource costs in another format which is more convenient for some scripts to access
	public Dictionary<Resource, int> ResourceCostsMap { get; protected set; }
	public List<Tech> techDependencies;
	public BuildProgressControl buildProgressObject;
	
	public string ControlCode {
		get { return "Create" + name; }
	}
	
	// Returns whether or not the given player meets all requirements to create this object.
	// Note: this function is called before the Creatable is instantiated
	public BoolAndString CanCreate(Player player) {
		if(ResourceCostsMap == null) {
			PopulateResourceCostsMap();
		}
		
		BoolAndString canCreate = new BoolAndString(true);
		if(gameObject.CompareTag("Tech")) {
			// Creatable is a tech, check to make sure the player hasn't already researched it
			if(player.PlayerStatus.techs.Contains(GetComponent<Tech>())) {
				canCreate.Bool = false;
				canCreate.String += "You have already researched this technology: "+GetComponent<Tech>();
			}
		}
		if(canCreate.Bool) {
			// Can still create, so continue checking tech dependencies
			foreach(Tech techDependency in techDependencies) {
				if(!player.PlayerStatus.techs.Contains(techDependency)) {
					canCreate.Bool = false;
					canCreate.String += "Player does not have the required technology: "+techDependency;
				}
			}
		}
		if(canCreate.Bool) {
			// Can still create, so continue checking resource levels
			foreach(ResourceAmount resourceCost in resourceCosts) {
				if(player.PlayerStatus.resourceLevels[resourceCost.resource] < resourceCost.amount) {
					canCreate.Bool = false;
					canCreate.String += "You do not have the required resource amount. Resource: "+resourceCost.resource+", Amount: "+resourceCost.amount+".";
				}
			}
		}
		return canCreate;
	}
	
	protected void PopulateResourceCostsMap() {
		ResourceCostsMap = new Dictionary<Resource, int>();
		foreach(ResourceAmount resourceAmount in resourceCosts) {
			ResourceCostsMap.Add(resourceAmount.resource, resourceAmount.amount);
		}
	}
	
	// Spends the resources required to create this object
	// Note: this function is called before the Creatable is instantiated
	public void SpendResources(Player player) {
		foreach(ResourceAmount resourceCost in resourceCosts) {
			player.PlayerStatus.SpendResource(resourceCost.resource, resourceCost.amount);
		}
	}
	
	void Start() {
		// Capture a Creatable's upkeep resources when it is instantiated
		Player owner = GetComponent<Controllable>().owner;
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				owner.PlayerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, this.gameObject);
			}
		}
	}
	
	// Called when the object is destroyed, this refunds and releases the Creatable's upkeep resources
	void OnDestroy() {
		Player owner = GetComponent<Controllable>().owner;
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				owner.PlayerStatus.GainResource(resourceCost.resource, resourceCost.amount);
				owner.PlayerStatus.ReleaseUpkeepResource(resourceCost.resource, this.gameObject);
			}
		}
	}
}