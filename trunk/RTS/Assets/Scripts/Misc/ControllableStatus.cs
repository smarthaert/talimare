using UnityEngine;
using System.Collections;

// Keeps information about a Controllable's current status
public class ControllableStatus : MonoBehaviour {

	public int maxHP;
	public int HP { get; protected set; }

	protected virtual void Start () {
		HP = maxHP;
	}
	
	protected virtual void Update () {}
	
	public void SetHPToZero() {
		HP = 0;
	}
	
	public void Damage(int amount) {
		HP -= amount;
		HP = Mathf.Clamp(HP, 0, maxHP);
		if(HP <= 0) {
			Die();
		}
	}
	
	public void Heal(int amount) {
		HP += amount;
		HP = Mathf.Clamp(HP, 0, maxHP);
	}
	
	protected void Die() {
		Destroy(this.gameObject);
	}
}

