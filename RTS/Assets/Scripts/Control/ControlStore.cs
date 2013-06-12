using UnityEngine;
using System.Collections.Generic;

public class ControlStore {
	
	//Tasks
	public const string STOP = "Stop";
	
	//Misc
	public const string TOGGLE_POWER = "TogglePower";
	public const string DESTROY = "Destroy";
	
	//Menu Navigation
	public const string MENU_BASE = "MenuBase";
	public const string MENU_CANCEL = "MenuCancel";
	public const string MENU_UNITS = "MenuUnits";
	public const string MENU_BUILDINGS = "MenuBuildings";
	public const string MENU_TECHS = "MenuTechs";
	public const string MENU_BACK = "MenuBack";
	public const string MENU_CONVERT_UNIT = "MenuConvert";
	
	public static Dictionary<string, Control> ControlMap { get; protected set; }
	
	static ControlStore() {
		ControlMap = new Dictionary<string, Control>();
		
		//Tasks
		ControlMap.Add(STOP, new Control("Stop", KeyCode.S));
		
		//Misc
		ControlMap.Add(TOGGLE_POWER, new Control("Toggle Power", KeyCode.P));
		ControlMap.Add(DESTROY, new Control("Destroy", KeyCode.Delete));
		
		//Menu Navigation
		ControlMap.Add(MENU_CANCEL, new Control("Cancel", KeyCode.Escape));
		ControlMap.Add(MENU_UNITS, new Control("Units", KeyCode.U));
		ControlMap.Add(MENU_BUILDINGS, new Control("Buildings", KeyCode.B));
		ControlMap.Add(MENU_TECHS, new Control("Techs", KeyCode.T));
		ControlMap.Add(MENU_BACK, new Control("Back", KeyCode.Escape));
		ControlMap.Add(MENU_CONVERT_UNIT, new Control("Convert", KeyCode.C));
		
		//Units
		ControlMap.Add("CreateCivilian", new Control("Civilian", KeyCode.C));
		
		//Buildings
		ControlMap.Add("CreateBase", new Control("Base", KeyCode.B));
		ControlMap.Add("CreatePower Plant", new Control("PowerPlant", KeyCode.P));
		
		//Techs
		ControlMap.Add("CreateElectricity", new Control("Electricity", KeyCode.E));
		ControlMap.Add("CreateCivHP", new Control("CivilianHP", KeyCode.H));
	}
}