using UnityEngine;
using System.Collections.Generic;

// Defines values for a Creatable object (unit, tech, or building)
public class Creatable : MonoBehaviour {
	
	public KeyCode creationKey;
	public int creationTime;
	public List<ResourceAmount> resourceCosts;
	
	private PlayerStatus playerStatus;
	
	void Awake () {
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent(typeof(PlayerStatus));
	}
	
	void Start () {
		// Ccapture a Creatable's upkeep resources when it is instantiated
		foreach(ResourceAmount resourceCost in resourceCosts) {
			if(resourceCost.IsUpkeepResource()) {
				playerStatus.CaptureUpkeepResource(resourceCost.resource, resourceCost.amount, this);
			}
		}
	}
}