using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(MoveTaskScript))]
[RequireComponent(typeof(AttackTaskScript))]
public class BaseUnitControl : Controllable {

	protected override void Start() {
		base.Start();
	}
	
	protected override void PopulateControlMenuList() {
		ControlMenu baseUnitMenu = new ControlMenu("baseUnit");
		baseUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.STOP, null));
		ControlMenuList.Add(baseUnitMenu);
		
		CurrentControlMenu = baseUnitMenu;
	}
	
	protected override void Update() {
		base.Update();
	}
	
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			AddTask(new Task(GetComponent<MoveTaskScript>(), hit.point), Game.PlayerInput.IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag("Unit")) {
			Controllable targetControl = hit.collider.gameObject.GetComponent<Controllable>();
			if(targetControl != null && owner != targetControl.owner && owner.relationships[targetControl.owner] == PlayerRelationship.HOSTILE) {
				AddTask(new Task(GetComponent<AttackTaskScript>(), targetControl.gameObject), Game.PlayerInput.IsMultiKeyPressed());
			}
		}
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
		
		if(controlCode.Equals(ControlStore.STOP)) {
			AbortTaskQueue();
		}
	}
}
