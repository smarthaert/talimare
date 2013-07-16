using UnityEngine;
using System.Collections;

// Keeps information about a Controllable's current status
public class ControllableStatus : MonoBehaviour {

	public int maxHP;
	public int HP { get; protected set; }
	
	protected virtual void Awake() {
		HP = maxHP;
	}

	protected virtual void Start() {}
	
	protected virtual void Update() {}
	
	public void SetHP(int hp) {
		HP = hp;
		HP = Mathf.Clamp(HP, 0, maxHP);
		if(HP <= 0) {
			Die();
		}
	}
	
	public void Damage(int amount) {
		SetHP(HP-amount);
	}
	
	public void Heal(int amount) {
		SetHP(HP+amount);
	}
	
	public void Die() {
		if(Game.PlayerInput.CurrentSelection == GetComponent<Selectable>()) {
			Game.PlayerInput.DeselectCurrent();
		}
		SendMessage("StopTask", SendMessageOptions.DontRequireReceiver);
		Destroy(this.gameObject);
	}
}

