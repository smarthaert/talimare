using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour {
	
	public Resource resource;
	public int amount;
	
	public void Gather(int amount) {
		this.amount -= amount;
		if(this.amount <= 0) {
			Die();
		}
	}
	
	public void Die() {
		Destroy(this.gameObject);
	}
}
