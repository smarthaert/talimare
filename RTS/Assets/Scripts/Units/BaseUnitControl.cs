using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[RequireComponent(typeof(MoveTaskScript))]
[RequireComponent(typeof(AttackTaskScript))]
public class BaseUnitControl : Controllable {
	
	protected override void BuildControlMenus() {
		ControlMenu baseUnitMenu = new ControlMenu();
		baseUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CONVERT_UNIT, ControlStore.MENU_CONVERT_UNIT));
		baseUnitMenu.MenuItems.Add(new ControlMenuItem(ControlStore.STOP));
		ControlMenus.Add(ControlStore.MENU_BASE, baseUnitMenu);
		
		ControlMenu convertMenu = new ControlMenu();
		foreach(CreatableUnit creatableUnit in GameUtil.GetAllCurrentUnitTypes(Owner)) {
			convertMenu.MenuItems.Add(new ControlMenuItem(creatableUnit, ControlStore.MENU_BASE));
		}
		ControlMenus.Add(ControlStore.MENU_CONVERT_UNIT, convertMenu);
		
		CurrentControlMenu = ControlMenus[ControlStore.MENU_BASE];
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
		} else {
			foreach(ControlMenuItem menuItem in ControlMenus[ControlStore.MENU_CONVERT_UNIT].MenuItems) {
				if(menuItem.Creatable.ControlCode.Equals(controlCode) && menuItem.Creatable.CanCreate(Owner).Bool) {
					BaseBuildingControl nearestBuilding = GameUtil.FindNearestBuildingToCreateUnit((CreatableUnit)menuItem.Creatable, this.transform.position, Owner);
					if(nearestBuilding != null) {
						nearestBuilding.QueueUnitToCreate(this, (CreatableUnit)menuItem.Creatable);
					} else {
						Debug.Log("No buildings are available to create that unit.");
					}
				}
			}
		}
	}
}
