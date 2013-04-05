using UnityEngine;
using System.Collections;

public class ResourceNode : SelectableControl {
	
	public Resource resource;
	public int amount;
	
	protected static AstarPath pathfinding;
	
	protected override void Start() {
		base.Start();
		if(pathfinding == null)
			pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
	}
	
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
		pathfinding.Scan();
	}
}