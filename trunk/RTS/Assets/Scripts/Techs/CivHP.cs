using UnityEngine;

public class CivHP : Tech {
	private static int bonusAmount = 20;
	
	public override void ApplyTechTo(GameObject gameObject) {
		Debug.Log("applying civHP to "+gameObject);
		gameObject.GetComponent<UnitStatus>().maxHP += bonusAmount;
		gameObject.GetComponent<UnitStatus>().Heal(20);
	}
}
