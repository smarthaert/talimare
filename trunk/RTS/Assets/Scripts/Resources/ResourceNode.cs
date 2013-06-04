using UnityEngine;
using System.Collections;

public class ResourceNode : Selectable {
	
	public Resource resource;
	
	public int startingAmount;
	public int CurrentAmount { get; protected set; }
	
	protected static AstarPath Pathfinding { get; set; }
	
	protected override void Awake() {
		base.Awake();
		
		CurrentAmount = startingAmount;
	}
	
	protected override void Start() {
		base.Start();
		
		if(Pathfinding == null) {
			Pathfinding = (AstarPath)GameObject.FindObjectOfType(typeof(AstarPath));
		}
	}
	
	public int GatherFrom(int maxAmount) {
		int actualAmount = Mathf.Min(maxAmount, CurrentAmount);
		CurrentAmount -= actualAmount;
		if(CurrentAmount <= 0) {
			Die();
		}
		return actualAmount;
	}
	
	public void Die() {
		Destroy(this.gameObject);
	}
	
	void OnDestroy() {
		Pathfinding.Scan();
	}
}
