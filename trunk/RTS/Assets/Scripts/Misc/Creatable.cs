using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, tech, or building)
public class Creatable : MonoBehaviour {
	
	public KeyCode creationKey;
	public float creationTime;
	public List<ResourceAmount> resourceCosts;
	public List<Tech> techDependencies;
	public BuildProgress buildProgressObject;
	
	protected Player player;
	
	// Returns whether or not the given player meets all requirements to create this object.
	// Note: this function is called before the Creatable is instantiated
	public bool CanCreate(Player player) {
		bool canCreate = true;
		if(gameObject.CompareTag("Tech")) {
			// Creatable is a tech, check to make sure the player hasn't already researched it
			if(player.playerStatus.techs.Contains(GetComponent<Tech>())) {
				canCreate = false;
				Debug.Log("Player has already researched this technology: "+GetComponent<Tech>());
			}
		}
		if(canCreate) {
			// Can still create, so continue checking resource levels
			foreach(ResourceAmount resourceCost in resourceCosts) {
				if(player.playerStatus.resourceLevels[resourceCost.resource] < resourceCost.amount) {
					canCreate = false;
					Debug.Log("Player does not have the required resource amount. Resource: "+resourceCost.resource+", Amount: "+resourceCost.amount+". Player has: "+player.playerStatus.resourceLevels[resourceCost.resource]);
					//show some nice error message to the player here
				}
			}
			foreach(Tech techDependency in techDependencies) {
				if(!player.playerStatus.techs.Contains(techDependency)) {
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
			player.playerStatus.SpendResource(resourceCost.resource, resourceCost.amount);
		}
	}
	
	void Start() {
		player = GetComponent<OwnedObjectControl>().player;
		
		// Capture a Creatable's upkeep resources when it is instantiated
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				Debug.Log("capturing upkeeps from player: "+player.id);
				player.playerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, this.gameObject);
			}
		}
	}
	
	// Called when the object is destroyed, this refunds and releases the Creatable's upkeep resources
	void OnDestroy() {
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				player.playerStatus.GainResource(resourceCost.resource, resourceCost.amount);
				player.playerStatus.ReleaseUpkeepResource(resourceCost.resource, this.gameObject);
			}
		}
	}
}