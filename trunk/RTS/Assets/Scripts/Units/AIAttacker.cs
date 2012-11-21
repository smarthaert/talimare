using UnityEngine;
using System.Collections;

// Handles unit attacking
public class AIAttacker : MonoBehaviour {
	
	protected GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		if(target != null) {
			//if not in range, make sure we're moving into range
			
			//if cooldown timer not running
				//if in range, engage target
				
				//if engaged, check range. if out of range, reset engage timer and move into range
				
				//if engage timer reaches limit, apply damage
					//then reset engage timer
					//then start cooldown timer
		}
	}
	
	public void Attack(GameObject target) {
		this.target = target;
	}
	
	public void StopAttacking() {
		target = null;
	}
}
