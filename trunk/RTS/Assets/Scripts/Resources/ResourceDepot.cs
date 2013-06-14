using UnityEngine;
using System.Collections.Generic;

// A resource depot allows a GameObject to receive collected resources, adding them to the owning player's supply
[RequireComponent(typeof(Controllable))]
[AddComponentMenu("Resources/Resource Depot")]
public class ResourceDepot : MonoBehaviour {

	public List<ResourceAmount> storableResources;
	protected Dictionary<Resource, int> StoredResources { get; set; }
	
	protected PlayerStatus PlayerStatus { get; set; }
	
	protected void Start() {
		PlayerStatus = GetComponent<Controllable>().Owner.PlayerStatus;
		
		StoredResources = new Dictionary<Resource, int>();
		foreach(ResourceAmount resourceAmount in storableResources) {
			StoredResources.Add(resourceAmount.resource, 0);
			DepositResource(resourceAmount.resource, resourceAmount.amount);
		}
	}
	
	public int GetResourceAmount(Resource resource) {
		return StoredResources[resource];
	}
	
	public void DepositResource(Resource resource, int amount) {
		StoredResources[resource] += amount;
		PlayerStatus.GainResource(resource, amount);
	}
	
	public int WithdrawResource(Resource resource, int maxAmount) {
		int actualAmount = Mathf.Min(maxAmount, StoredResources[resource]);
		StoredResources[resource] -= actualAmount;
		PlayerStatus.LoseResource(resource, actualAmount);
		return actualAmount;
	}
	
	// Returns the nearest ResourceDepot to the given point which is owned by the given player and can store the given resource
	public static ResourceDepot FindNearestDepotForResource(Resource resource, Player player, Vector3 point) {
		ResourceAmount resourceAsAmount = new ResourceAmount(resource, 0);
		float minDist = Mathf.Infinity;
		ResourceDepot minDepot = null;
		foreach(ResourceDepot resourceDepot in GameUtil.FindAllOwnedInstancesOf<ResourceDepot>(player)) {
			if(resourceDepot.storableResources.Contains(resourceAsAmount)) {
				float dist = Vector3.Distance(point, resourceDepot.transform.position);
				if(dist < minDist) {
					minDist = dist;
					minDepot = resourceDepot;
				}
			}
		}
		return minDepot;
	}
	
	// Returns the nearest ResourceDepot to the given point which is owned by the given player and has some amount of the given resource stored
	public static ResourceDepot FindNearestDepotWithResource(Resource resource, Player player, Vector3 point) {
		ResourceAmount resourceAsAmount = new ResourceAmount(resource, 0);
		float minDist = Mathf.Infinity;
		ResourceDepot minDepot = null;
		foreach(ResourceDepot resourceDepot in GameUtil.FindAllOwnedInstancesOf<ResourceDepot>(player)) {
			if(resourceDepot.storableResources.Contains(resourceAsAmount) && resourceDepot.StoredResources[resource] > 0) {
				float dist = Vector3.Distance(point, resourceDepot.transform.position);
				if(dist < minDist) {
					minDist = dist;
					minDepot = resourceDepot;
				}
			}
		}
		return minDepot;
	}
	
	// Returns all ResourceDepots which are owned by the given player and have some amount of the given resource stored
	public static List<ResourceDepot> FindAllDepotsWithResource(Resource resource, Player player) {
		ResourceAmount resourceAsAmount = new ResourceAmount(resource, 0);
		List<ResourceDepot> allDepots = new List<ResourceDepot>();
		foreach(ResourceDepot resourceDepot in GameUtil.FindAllOwnedInstancesOf<ResourceDepot>(player)) {
			if(resourceDepot.storableResources.Contains(resourceAsAmount) && resourceDepot.StoredResources[resource] > 0) {
				allDepots.Add(resourceDepot);
			}
		}
		return allDepots;
	}
}
