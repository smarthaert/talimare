using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class UnitControl : OwnedObjectControl {
	
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
			//CommandHandler.AddCommandFromLocal(new MoveCommand(OwnedObjectId, hit.point));
			//TODO queue move action
		} else if(hit.collider.gameObject.CompareTag("Unit") && hit.collider.gameObject.GetComponent<OwnedObjectControl>() != null
				//TODO check player relationship
				) {
			//CommandHandler.AddCommandFromLocal(new AttackCommand(OwnedObjectId, hit.collider.gameObject.GetComponent<OwnedObjectControl>().OwnedObjectId));
			//TODO queue attack action
		}
	}
	
	// Called when any key is pressed while this unit is selected
	public override void KeyPressed() {
		if(Input.GetKeyDown(KeyCode.S)) {
			SendMessage("StopAllActions");
			//TODO empty action queue
		}
	}
	
	public void ExecuteMove(Vector3 target) {
		SendMessage("StopAllActions");
		pathfinder.Move(target);
	}
	
	public void ExecuteAttack(GameObject gameObject) {
		SendMessage("StopAllActions");
		attacker.Attack(gameObject);
	}
	
	// Stops all actions the unit is performing. Keep in mind that it's likely that one of the stopped actions will be resumed immediately
	public virtual void StopAllActions() {
		attacker.StopAttacking();
		pathfinder.StopMoving();
	}
	
	// Called when another object moves into visual range
	public virtual void ObjectEnteredVision(GameObject obj) {
		
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		
	}
}