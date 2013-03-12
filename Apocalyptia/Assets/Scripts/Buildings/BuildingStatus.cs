using UnityEngine;
using System.Collections;

// Keeps information about a building's current status
public class BuildingStatus : MonoBehaviour {
	
	public int maxHP;
	protected int currentHP;

	void Start () {
		currentHP = maxHP;
	}
	
	void Update () {
		
	}
	
	public int getCurrentHP() {
		return currentHP;
	}
	
	public void Damage(int amount) {
		currentHP -= amount;
		Mathf.Clamp(currentHP, 0, maxHP);
		if(currentHP <= 0) {
			Die();
		}
	}
	
	public void Heal(int amount) {
		currentHP += amount;
		Mathf.Clamp(currentHP, 0, maxHP);
	}
	
	protected void Die() {
		Destroy(this);
	}
}
