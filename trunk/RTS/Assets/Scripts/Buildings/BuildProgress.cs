using UnityEngine;
using System;

// Handles the status of a building being placed and built
public class BuildProgress : MonoBehaviour {
	
	public GameObject finishedObject;
	
	protected static AstarPath pathfinding;
	[NonSerialized]
	public Creatable creatable;
	[NonSerialized]
	public Player player;
	
	// The amount of time that has been spent creating this building
	protected float timeSpentCreating = 0;
	// Whether or not this building was completed (this is probably only available for one frame before this GameObject is destroyed)
	protected bool completed = false;
	
	void Start() {
		creatable = finishedObject.GetComponent<Creatable>();
	}
	
	// Called when this building is committed (goes from a queued/placement state to actually being in the world)
	public void Commit() {
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		pathfinding.Scan();
		creatable.SpendResources(player);
	}
	
	// Called at regular intervals while this building is being built to advance its completion
	public void Building(float timeSpent) {
		timeSpentCreating += timeSpent;
		Mathf.Clamp(timeSpentCreating, 0, creatable.creationTime);
		if(timeSpentCreating >= creatable.creationTime) {
			Complete();
		}
	}
	
	// Complete the building, instantiating its finished version, giving it a player, and destroying this progress object
	protected void Complete() {
		completed = true;
		GameObject newBuilding = (GameObject)Instantiate(finishedObject, this.transform.position, this.transform.rotation);
		newBuilding.GetComponent<Creatable>().player = player;
		Destroy(this.gameObject);
	}
	
	// Returns the creation percentage complete as an integer
	public int PercentComplete() {
		return Mathf.FloorToInt(timeSpentCreating / creatable.creationTime);
	}
	
	public bool Completed() {
		return completed;
	}
}

