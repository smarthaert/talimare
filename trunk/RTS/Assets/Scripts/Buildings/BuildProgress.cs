using UnityEngine;

// Handles the status of a building being built
public class BuildProgress : MonoBehaviour {
	
	public GameObject finishedObject;
	
	protected static AstarPath pathfinding;
	protected Creatable creatable;
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0;
	// Whether or not this building was completed (this is probably only available for one frame before this GameObject is destroyed)
	protected bool completed = false;
	
	void Start() {
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		pathfinding.Scan();
		creatable = finishedObject.GetComponent<Creatable>();
	}
	
	// Called at regular intervals while this building is being built to advance its completion
	public void Building(float timeSpent) {
		timeSpentCreating += timeSpent;
		Mathf.Clamp(timeSpentCreating, 0, creatable.creationTime);
		if(timeSpentCreating >= creatable.creationTime) {
			completed = true;
			Instantiate(finishedObject, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / creatable.creationTime);
	}
	
	public bool Completed() {
		return completed;
	}
}

