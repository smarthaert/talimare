using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, building, or tech)
public class Creatable : MonoBehaviour {
	
	public float creationTime;
	public List<ResourceAmount> resourceCosts;
	public List<Tech> techDependencies;
	public BuildProgressControl buildProgressObject;
	
	public string ControlCode {
		get { return "Create"+this.name; }
	}
	
	// Returns whether or not the given player meets all requirements to create this object.
	// Note: this function is called before the Creatable is instantiated
	public bool CanCreate(Player player) {
		bool canCreate = true;
		if(gameObject.CompareTag("Tech")) {
			// Creatable is a tech, check to make sure the player hasn't already researched it
			if(player.PlayerStatus.techs.Contains(GetComponent<Tech>())) {
				canCreate = false;
				Debug.Log("Player has already researched this technology: "+GetComponent<Tech>());
			}
		}
		if(canCreate) {
			// Can still create, so continue checking resource levels
			foreach(ResourceAmount resourceCost in resourceCosts) {
				if(player.PlayerStatus.resourceLevels[resourceCost.resource] < resourceCost.amount) {
					canCreate = false;
					Debug.Log("Player does not have the required resource amount. Resource: "+resourceCost.resource+", Amount: "+resourceCost.amount+". Player has: "+player.PlayerStatus.resourceLevels[resourceCost.resource]);
					//show some nice error message to the player here
				}
			}
			foreach(Tech techDependency in techDependencies) {
				if(!player.PlayerStatus.techs.Contains(techDependency)) {
					canCreate = false;
					Debug.Log("Player does not have the required technology: "+techDependency);
				}
			}
		}
		return canCreate;
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