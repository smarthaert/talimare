using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(AIPathfinder))]
[RequireComponent(typeof(AIAttacker))]
public class UnitControl : SelectableControl {
	
	protected AIPathfinder pathfinder;
	protected AIAttacker attacker;
	protected Player player;

	protected override void Start() {
		base.Start();
		
		pathfinder = GetComponent<AIPathfinder>();
		attacker = GetComponent<AIAttacker>();
		player = GetComponent<Creatable>().player;
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Called when mouse action button is clicked on any object while this unit is selected
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			Debug.Log ("adding move command, object: "+objectId+"hit: "+hit.point);
			CommandHandler.AddCommandFromLocal(new MoveCommand(objectId, hit.point));
		} else if(hit.collider.gameObject.CompareTag("Unit") && hit.collider.gameObject.GetComponent<Creatable>() != null && 
				hit.collider.gameObject.GetComponent<Creatable>().player.team != PlayerHub.myPlayer.team) {
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
	
	// Called when an enemy object moves into visual range
	public virtual void EnemyEnteredVision(GameObject obj) {
		
	}
	
	// Called when an oenemy object moves out of visual range
	public virtual void EnemyLeftVision(GameObject obj) {
		
	}
}
