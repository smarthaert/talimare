using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class BaseUnitControl : Controllable {

	protected override void Start() {
		base.Start();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			AddAction(new MoveAction(this, hit.point), IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag("Unit")) {
			Controllable targetControl = hit.collider.gameObject.GetComponent<Controllable>();
			if(targetControl != null && owner != targetControl.owner && owner.relationships[targetControl.owner] == PlayerRelationship.HOSTILE) {
				AddAction(new AttackAction(this, targetControl), IsMultiKeyPressed());
			}
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		if(Input.GetKeyDown(KeyCode.S)) {
			AbortActionQueue();
		}
	}
}
