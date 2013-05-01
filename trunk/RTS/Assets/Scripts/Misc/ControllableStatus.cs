using UnityEngine;
using System.Collections;

// Keeps information about a Controllable's current status
public class ControllableStatus : MonoBehaviour {

	public int maxHP;
	public int HP { get; protected set; }
	public bool IsAlive { get; protected set; }

	protected virtual void Start () {
		HP = maxHP;
		IsAlive = true;
	}
	
	protected virtual void Update () {}
	
	public void SetHPToZero() {
		HP = 0;
	}
	
	public void Damage(int amount) {
		HP -= amount;
		Mathf.Clamp(HP, 0, maxHP);
		if(HP == 0) {
			Die();
		}
	}
	
	public void Heal(int amount) {
		HP += amount;
		Mathf.Clamp(HP, 0, maxHP);
	}
	
	protected void Die() {
		IsAlive = false;
		Destroy(this.gameObject);
	}
}

