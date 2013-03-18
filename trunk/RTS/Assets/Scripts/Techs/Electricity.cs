using UnityEngine;
using System.Collections;

public class Electricity : Tech {
	
	public override void ApplyTechTo(GameObject gameObject) {
		Debug.Log("applying electricity to "+gameObject);
	}
}
