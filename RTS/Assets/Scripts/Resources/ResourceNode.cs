using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour {
	
	public Resource resource;
	public int amount;
	
	protected static AstarPath pathfinding;
	
	public void Gather(int amount) {
		this.amount -= amount;
		if(this.amount <= 0) {
			Die();
		}
	}
	
	public void Die() {
		Destroy(this.gameObject);
	}
	
	void OnDestroy() {
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		pathfinding.Scan();
	}
}
