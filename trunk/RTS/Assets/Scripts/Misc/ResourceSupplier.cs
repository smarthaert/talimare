using UnityEngine;
using System.Collections.Generic;

// Defines a GameObject whose existence supplies an amount of a resource
public class ResourceSupplier : MonoBehaviour {
	
	public List<ResourceAmount> suppliedResources;
	
	private static PlayerStatus playerStatus;

	void Start () {
		if(playerStatus == null)
			playerStatus = GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
		
		// Add all supplied resources to the player's pool
		foreach(ResourceAmount suppliedResource in suppliedResources) {
			playerStatus.GainResource(suppliedResource.resource, suppliedResource.amount);
			if(suppliedResource.IsUpkeepResource()) {
				playerStatus.AddSuppliedUpkeepResource(suppliedResource.resource, suppliedResource.amount);
			}
		}
	}
	
	void Update () {}
	
	// Called when this object is destroyed. Removes the provided resource from the player's pool
	void OnDestroy () {
		foreach(ResourceAmount suppliedResource in suppliedResources) {
			if(suppliedResource.IsUpkeepResource()) {
				playerStatus.RemoveSuppliedUpkeepResource(suppliedResource.resource, suppliedResource.amount);
			}
		}
	}
}
