using UnityEngine;
using System.Collections;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
public class UnitControl : SelectableControl {
	
	protected AIPathfinder pathfinder;
	
	protected override void Awake() {
		base.Awake();
		
		pathfinder = GetComponent<AIPathfinder>();
	}

	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		pathfinder.MoveTo(hit.point);
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		
	}
}
