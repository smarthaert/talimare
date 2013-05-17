using UnityEngine;
using System.Collections.Generic;

// Defines a GameObject whose existence supplies an amount of a resource
public class ResourceSupplier : MonoBehaviour {
	
	public List<ResourceAmount> suppliedResources;
	
	protected Player player;

	protected void Start () {
		player = GetComponent<Controllable>().Owner;
		
		// Add all supplied resources to the player's pool
		foreach(ResourceAmount suppliedResource in suppliedResources) {
			player.PlayerStatus.GainResource(suppliedResource.resource, suppliedResource.amount);
			if(suppliedResource.IsUpkeepResource()) {
				player.PlayerStatus.AddSuppliedUpkeepResource(suppliedResource.resource, suppliedResource.amount);
			}
		}
	}
	
	// Called when this object is destroyed. Removes the provided resource from the player's pool
	protected void OnDestroy () {
		foreach(ResourceAmount suppliedResource in suppliedResources) {
			if(suppliedResource.IsUpkeepResource()) {
				player.PlayerStatus.LoseResource(suppliedResource.resource, suppliedResource.amount);
				player.PlayerStatus.RemoveSuppliedUpkeepResource(suppliedResource.resource, suppliedResource.amount);
			}
		}
	}
}
