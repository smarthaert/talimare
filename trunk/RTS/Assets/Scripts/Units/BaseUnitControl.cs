using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

// Contains general unit utility functions
[AddComponentMenu("Units/Base Unit Control")]
public class BaseUnitControl : Controllable {
	
	protected virtual void BuildControlMenus() {
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CONVERT_UNIT, ControlStore.MENU_CONVERT_UNIT));
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.STOP));
		
		ControlMenu convertMenu = new ControlMenu();
		foreach(CreatableUnit creatableUnit in GameUtil.GetAllCurrentUnitTypes(Owner)) {
			convertMenu.MenuItems.Add(new ControlMenuItem(creatableUnit, ControlStore.MENU_BASE));
		}
		ControlMenus.Add(ControlStore.MENU_CONVERT_UNIT, convertMenu);
	}
	
	protected override void Update() {
		base.Update();
	}
	
	public override void ReceiveMouseAction(RaycastHit hit) {
		base.ReceiveMouseAction(hit);
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
