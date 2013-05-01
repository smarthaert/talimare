using UnityEngine;
using System.Collections.Generic;

public class ControlMenu {
	public List<ControlMenuItem> MenuItems { get; protected set; }
	
	public ControlMenu() {
		MenuItems = new List<ControlMenuItem>();
	}
	
	public ControlMenuItem GetMenuItemWithCode(string controlCode) {
		foreach(ControlMenuItem menuItem in MenuItems) {
			if(menuItem.ControlCode.Equals(controlCode)) {
				return menuItem;
			}
		}
		return null;
	}
}