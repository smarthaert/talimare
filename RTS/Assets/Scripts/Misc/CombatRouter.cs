using UnityEngine;
using System.Collections.Generic;

public static class CombatRouter {

	public static void DealDamage(GameObject source, GameObject target, int amount) {
		target.GetComponent<ControllableStatus>().Damage(amount);
		if(target.GetComponent<PersonalAI>() != null) {
			target.GetComponent<PersonalAI>().TookDamage(source);
		}
	}
}

