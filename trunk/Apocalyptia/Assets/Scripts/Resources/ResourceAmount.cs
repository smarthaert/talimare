using System;

// Convenience class for storing a resource and an amount together
[System.Serializable]
public class ResourceAmount {
	public Resource resource;
	public int amount;
	
	// Convenience method to determine if this resource is an upkeep resource
	public bool IsUpkeepResource() {
		if(resource == Resource.Food || resource == Resource.Water || resource == Resource.Power)
			return true;
		else
			return false;
	}
}

