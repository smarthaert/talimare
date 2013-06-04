using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(MoveTaskScript))]
[RequireComponent(typeof(AttackTaskScript))]
public class BaseUnitControl : Controllable {
	
	protected override void BuildControlMenus() {
		ControlMenu baseUnitMenu = new ControlMenu();
		baseUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.STOP));
		ControlMenus.Add(BASE_MENU_NAME, baseUnitMenu);
		
		CurrentControlMenu = ControlMenus[BASE_MENU_NAME];
	}
	
	protected override void Update() {
		base.Update();
	}
	
	public override void MouseAction(RaycastHit hit) {
		if(hit.collider.GetType() == typeof(TerrainCollider)) {
			AddTask(new MoveTask(GetComponent<MoveTaskScript>(), hit.point), Game.PlayerInput.IsMultiKeyPressed());
		} else if(hit.collider.gameObject.CompareTag(GameUtil.TAG_UNIT)) {
			Controllable targetControl = hit.collider.gameObject.GetComponent<Controllable>();
			if(targetControl != null && Owner != targetControl.Owner && Owner.Relationships[targetControl.Owner] == PlayerRelationship.HOSTILE) {
				AddTask(new AttackTask(GetComponent<AttackTaskScript>(), targetControl.gameObject), Game.PlayerInput.IsMultiKeyPressed());
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
