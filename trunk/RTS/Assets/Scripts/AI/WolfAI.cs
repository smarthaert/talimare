using UnityEngine;
using System.Collections;

[AddComponentMenu("AI/Wolf AI")]
public class WolfAI : PersonalAI {

	protected override void ContinueIdling() {
		base.ContinueIdling();
		//TODO low: wander
	}
}

