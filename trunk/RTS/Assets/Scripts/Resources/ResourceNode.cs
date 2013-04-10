using UnityEngine;
using System.Collections;

public class ResourceNode : Selectable {
	
	public Resource resource;
	
	public int startingAmount;
	public int CurrentAmount { get; protected set; }
	
	protected static AstarPath Pathfinding { get; set; }
	
	protected override void Start() {
		base.Start();
		CurrentAmount = startingAmount;
		if(Pathfinding == null)
			Pathfinding = (AstarPath)GameObject.FindObjectOfType(typeof(AstarPath));
	}
	
	public int GatherFrom(int amount) {
		CurrentAmount -= amount;
		if(CurrentAmount <= 0) {
			Die();
			return amount + CurrentAmount;
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
