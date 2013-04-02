using UnityEngine;
using System.Collections.Generic;

// A resource depot allows a GameObject to receive collected resources, adding them to the owning player's supply
[RequireComponent(typeof(Controllable))]
public class ResourceDepot : MonoBehaviour {

	public List<Resource> storableResources;
	
	protected PlayerStatus PlayerStatus { get; set; }
	
	protected void Start() {
		PlayerStatus = GetComponent<Controllable>().owner.PlayerStatus;
	}
	
	public void DepositResource(Resource resource, int amount) {
		PlayerStatus.GainResource(resource, amount);
	}
	
	public static ResourceDepot FindNearestDepotForResource(Vector3 point, Player player, Resource resource) {
		float minDist = Mathf.Infinity;
		ResourceDepot minComp = null;
		ResourceDepot[] components = (ResourceDepot[])GameObject.FindObjectsOfType(typeof(ResourceDepot));
		foreach(ResourceDepot component in components) {
			if(component.storableResources.Contains(resource) && component.GetComponent<Controllable>().owner == player) {
				float dist = Vector3.Distance(point, component.transform.position);
				if(dist < minDist) {
					minComp = component;
				}
			}
		}
		return minComp;
	}
}
