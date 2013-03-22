using UnityEngine;
using System.Collections;

// Keeps information about a unit's current status
public class UnitStatus : MonoBehaviour {
	
	public int maxHP;
	protected int currentHP;
	
	protected TextMesh hpText;

	void Start () {
		currentHP = maxHP;
		
		hpText = GetComponentInChildren<TextMesh>();
	}
	
	void Update () {
		hpText.text = currentHP.ToString();
	}
	
	public int GetCurrentHP() {
		return currentHP;
	}
	
	public void SetCurrentHP(int amount) {
		currentHP = amount;
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
		Destroy(this.gameObject);
	}
}
