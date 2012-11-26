using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class UnitControl : SelectableControl {
	
	protected PlayerStatus playerStatus;
	protected AIPathfinder pathfinder;
	protected AIAttacker attacker;

	protected override void Start() {
		base.Start();
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
		
		pathfinder = GetComponent<AIPathfinder>();
		attacker = GetComponent<AIAttacker>();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			StopAllActions();
			attacker.StopAttacking();
			pathfinder.Move(hit.point);
			//TODO stop units from walking on top of each other
		} else if(hit.collider.gameObject.CompareTag("Unit")) {
			StopAllActions();
			attacker.Attack(hit.collider.gameObject);
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		
	}
	
	// Overridden in subclasses so this class can request all subclass actions to be stopped
	public virtual void StopAllActions() {}
}
