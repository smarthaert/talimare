using UnityEngine;

// Handles the status of a builder (a building being built)
public class Builder : MonoBehaviour {
	
	public GameObject actualBuilding;
	
	protected Creatable creatable;
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0;
	// Whether or not this building was created (this is probably only available for one frame before the builder is destroyed)
	protected bool complete = false;
	
	void Start() {
		creatable = GetComponent<Creatable>();
	}
	
	// Called at regular intervals while this building is being built
	public void Building(float timeSpent) {
		timeSpentCreating += timeSpent;
		Mathf.Clamp(timeSpentCreating, 0, creatable.creationTime);
		if(timeSpentCreating >= creatable.creationTime) {
			complete = true;
			Debug.Log(this+" built!");
			Instantiate(actualBuilding, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / creatable.creationTime);
	}
	
	public bool IsComplete() {
		return complete;
	}
}

