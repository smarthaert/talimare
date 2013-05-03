using System;

// Convenience class for storing a resource and an amount together
[System.Serializable]
public class ResourceAmount {
	public Resource resource;
	public int amount;
	
	public ResourceAmount() {}
	
	public ResourceAmount(Resource resource, int amount) {
		this.resource = resource;
		this.amount = amount;
	}
	
	// Convenience method to determine if this resource is an upkeep resource
	public bool IsUpkeepResource() {
		if(resource == Resource.Food)
			return true;
		else
			return false;
	}
}

