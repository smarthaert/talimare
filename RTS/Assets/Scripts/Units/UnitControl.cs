using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class UnitControl : SelectableControl {
	
	protected AIPathfinder pathfinder;
	protected AIAttacker attacker;

	protected override void Start() {
		base.Start();
		
		pathfinder = GetComponent<AIPathfinder>();
		attacker = GetComponent<AIAttacker>();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			SendMessage("StopAllActions");
			pathfinder.Move(hit.point);
			//TODO stop units from walking on top of each other
		} else if(hit.collider.gameObject.CompareTag("Unit")) {
			SendMessage("StopAllActions");
			attacker.Attack(hit.collider.gameObject);
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		if(Input.GetKeyDown(KeyCode.S)) {
			SendMessage("StopAllActions");
		}
	}
	
	// Stops all actions the unit is performing. Keep in mind that it's likely that one of the stopped actions will be resumed immediately
	public virtual void StopAllActions() {
		attacker.StopAttacking();
		pathfinder.StopMoving();
	}
}
