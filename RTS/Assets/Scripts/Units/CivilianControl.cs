using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Units/Civilian Control")]
public class CivilianControl : BaseUnitControl {
	
	protected override void Start() {
		base.Start();
	}
	
	protected override void BuildControlMenus() {
		base.BuildControlMenus();
		
		ControlMenus[ControlStore.MENU_BASE].MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BUILDINGS, ControlStore.MENU_BUILDINGS));
		
		ControlMenu createBuildingMenu = new ControlMenu();
		foreach(CreatableBuilding building in Game.PlayerInput.buildings) {
			createBuildingMenu.MenuItems.Add(new ControlMenuItem(building, ControlStore.MENU_CANCEL));
		}
		createBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		ControlMenus.Add(ControlStore.MENU_BUILDINGS, createBuildingMenu);
		
		ControlMenu cancelCreateMenu = new ControlMenu();
		cancelCreateMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CANCEL, ControlStore.MENU_BUILDINGS));
		ControlMenus.Add(ControlStore.MENU_CANCEL, cancelCreateMenu);
	}
	
	protected override void Update() {
		base.Update();
	}
	
	public override void ReceiveMouseAction(RaycastHit hit) {
		base.ReceiveMouseAction(hit);
	}
	
	public override void ReceiveControlCode(string controlCode) {
		base.ReceiveControlCode(controlCode);
	}
}
