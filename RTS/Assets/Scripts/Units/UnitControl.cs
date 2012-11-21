using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class UnitControl : SelectableControl {
	
	public bool attackWhileMoving = false;
	
	protected AIPathfinder pathfinder;
	protected AIAttacker attacker;
	
	protected override void Awake() {
		base.Awake();
		
		pathfinder = GetComponent<AIPathfinder>();
		attacker = GetComponent<AIAttacker>();
	}

	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			if(!attackWhileMoving)
				attacker.StopAttacking();
			pathfinder.Move(hit.point);
		} else if(hit.collider.GetType() == typeof(CharacterController)) {
			if(!attackWhileMoving)
				pathfinder.StopMoving();
			attacker.Attack(hit.collider.gameObject);
		}
	}
	
	// Can be called from elsewhere to order this unit to move
	public void MoveTo(Vector3 destination) {
		pathfinder.Move(destination);
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		
	}
}
