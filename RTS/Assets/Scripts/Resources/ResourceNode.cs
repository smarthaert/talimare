using UnityEngine;
using System.Collections;

public class ResourceNode : Selectable {
	
	public Resource resource;
	public int amount;
	
	protected static AstarPath Pathfinding { get; set; }
	
	protected override void Start() {
		base.Start();
		if(Pathfinding == null)
			Pathfinding = (AstarPath)GameObject.FindObjectOfType(typeof(AstarPath));
	}
	
	public int GatherFrom(int amount) {
		this.amount -= amount;
		if(this.amount <= 0) {
			Die();
			return amount + this.amount;
		}
		return amount;
	}
	
	public void Die() {
		Destroy(this.gameObject);
	}
	
	void OnDestroy() {
		Pathfinding.Scan();
	}
}
