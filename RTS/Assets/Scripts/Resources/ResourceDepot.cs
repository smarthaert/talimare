using UnityEngine;
using System.Collections.Generic;

// A resource depot allows a GameObject to receive collected resources, adding them to the owning player's supply
[RequireComponent(typeof(Controllable))]
public class ResourceDepot : MonoBehaviour {

	public List<Resource> storableResources;
	protected Dictionary<Resource, int> StoredResources { get; set; }
	
	protected PlayerStatus PlayerStatus { get; set; }
	
	protected void Start() {
		PlayerStatus = GetComponent<Controllable>().owner.PlayerStatus;
		
		StoredResources = new Dictionary<Resource, int>();
		foreach(Resource resource in storableResources) {
			StoredResources.Add(resource, 0);
		}
	}
	
	public int GetResourceAmount(Resource resource) {
		return StoredResources[resource];
	}
	
	public void DepositResource(Resource resource, int amount) {
		StoredResources[resource] += amount;
		PlayerStatus.GainResource(resource, amount);
	}
	
	public void WithdrawResource(Resource resource, int amount) {
		StoredResources[resource] -= amount;
		PlayerStatus.GainResource(resource, amount);
	}
	
	// Returns the nearest ResourceDepot to the given point which is owned by the given player and can store the given resource
	public static ResourceDepot FindNearestDepotForResource(Vector3 point, Player player, Resource resource) {
		float minDist = Mathf.Infinity;
		ResourceDepot minComp = null;
		foreach(ResourceDepot resourceDepot in GameUtil.FindAllOwnedInstancesOf<ResourceDepot>(player)) {
			if(resourceDepot.storableResources.Contains(resource)) {
				float dist = Vector3.Distance(point, resourceDepot.transform.position);
				if(dist < minDist) {
					minDist = dist;
					minComp = resourceDepot;
				}
			}
		}
		return minComp;
	}
	
	// Returns the nearest ResourceDepot to the given point which is owned by the given player and has some amount of the given resource stored
	public static ResourceDepot FindNearestDepotWithResource(Vector3 point, Player player, Resource resource) {
		float minDist = Mathf.Infinity;
		ResourceDepot minComp = null;
		foreach(ResourceDepot resourceDepot in GameUtil.FindAllOwnedInstancesOf<ResourceDepot>(player)) {
			if(resourceDepot.storableResources.Contains(resource) && resourceDepot.StoredResources[resource] > 0) {
				float dist = Vector3.Distance(point, resourceDepot.transform.position);
				if(dist < minDist) {
					minDist = dist;
					minComp = resourceDepot;
				}
			}
		}
		return minComp;
	}
}
