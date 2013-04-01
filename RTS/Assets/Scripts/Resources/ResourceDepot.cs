using UnityEngine;
using System.Collections.Generic;

// A resource depot allows a GameObject to receive collected resources, adding them to the owning player's supply
[RequireComponent(typeof(Controllable))]
public class ResourceDepot : MonoBehaviour {

	public List<Resource> storableResources;
	
	protected PlayerStatus PlayerStatus { get; set; }
	
	protected void Awake() {
		PlayerStatus = GetComponent<Controllable>().owner.PlayerStatus;
	}
	
	protected void StoreResource(Resource resource, int amount) {
		PlayerStatus.GainResource(resource, amount);
	}
}
