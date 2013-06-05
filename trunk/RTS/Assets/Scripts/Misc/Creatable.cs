using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, building, or tech)
[RequireComponent(typeof(Controllable))]
public abstract class Creatable : MonoBehaviour {
	
	public float creationTime;
	public List<ResourceAmount> resourceCosts;
	// Holds the resource costs in another format which is more convenient for some scripts to access
	public Dictionary<Resource, int> ResourceCostsMap { get; protected set; }
	public List<Tech> techDependencies;
	
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
		if(gameObject.CompareTag(GameUtil.TAG_TECH)) {
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
			if(resourceCost.IsUpkeepResource()) {
				player.PlayerStatus.LoseResource(resourceCost.resource, resourceCost.amount);
			}
		}
	}
	
	protected void Start() {
		// Capture a Creatable's upkeep resources when it is instantiated
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				GetComponent<Controllable>().Owner.PlayerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, this.gameObject);
			}
		}
	}
	
	// Called when the object is destroyed, this refunds and releases the Creatable's upkeep resources
	protected void OnDestroy() {
		Player owner = GetComponent<Controllable>().Owner;
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				owner.PlayerStatus.GainResource(resourceCost.resource, resourceCost.amount);
				owner.PlayerStatus.ReleaseUpkeepResource(resourceCost.resource, this.gameObject);
			}
		}
	}
}