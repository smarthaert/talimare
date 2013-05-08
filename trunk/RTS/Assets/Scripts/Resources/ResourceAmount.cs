using System;

// Convenience class for storing a resource and an amount together. Basically used only to allow resource+amount choices in the Inspector
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
	
	// Two ResourceAmounts are considered equal simply if they contain the same Resource
	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(ResourceAmount))
			return false;
		ResourceAmount other = (ResourceAmount)obj;
		return resource == other.resource;
	}


	public override int GetHashCode() {
		unchecked {
			return resource.GetHashCode();
		}
	}

}

