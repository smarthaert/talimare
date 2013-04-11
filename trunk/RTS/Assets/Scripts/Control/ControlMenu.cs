using UnityEngine;
using System.Collections.Generic;

public class ControlMenu {
	public string Name { get; protected set; }
	public List<ControlMenuItem> MenuItems { get; protected set; }
	
	public ControlMenu(string name) {
		Name = name;
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