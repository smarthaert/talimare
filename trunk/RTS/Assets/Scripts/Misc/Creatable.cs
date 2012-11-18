using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, tech, or building)
public class Creatable : MonoBehaviour {
	
	public KeyCode creationKey;
	public int creationTime;
	public List<ResourceAmount> resourceCosts;
	public List<Tech> techDependencies;
	
	private static PlayerStatus playerStatus;
	
	void Awake () {
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent(typeof(PlayerStatus));
	}
	
	void Start () {
		// Capture a Creatable's upkeep resources when it is instantiated
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				playerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, this.gameObject);
			}
		}
	}
	
	// Returns whether or not all requirements are met to create this object
	public bool CanCreate() {
		bool canCreate = true;
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(playerStatus.resourceLevels[resourceCost.resource] < resourceCost.amount) {
				canCreate = false;
				Debug.Log("Player does not have the required resource amount. Resource: "+resourceCost.resource+", Amount: "+resourceCost.amount+". Player has: "+playerStatus.resourceLevels[resourceCost.resource]);
				//show some nice error message to the player here
			}
		}
		foreach(Tech techDependency in techDependencies) {
			if(!playerStatus.techs.Contains(techDependency)) {
				canCreate = false;
				Debug.Log("Player does not have the required technology: "+techDependency);
			}
		}
		return canCreate;
	}
	
	// Spends the resources required to create this object
	public void SpendResources() {
		foreach(ResourceAmount resourceCost in resourceCosts) {
			playerStatus.SpendResource(resourceCost.resource, resourceCost.amount);
		}
	}
}