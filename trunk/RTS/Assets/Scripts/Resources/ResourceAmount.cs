using System;

[System.Serializable]
public class ResourceAmount {
	public Resource resource;
	public int amount;
	
	public bool IsUpkeepResource() {
		if(resource == Resource.Food || resource == Resource.Water || resource == Resource.Power)
			return true;
		else
			return false;
	}
}

