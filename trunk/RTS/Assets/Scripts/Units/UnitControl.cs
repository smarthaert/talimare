using UnityEngine;
using System.Collections;

// Contains general unit utility functions
public class UnitControl : SelectableControl {

	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		
	}
}
