using UnityEngine;

public class Control {
	public string Name { get; set; }
	public KeyCode Hotkey { get; set; }
	
	public Control(string name, KeyCode hotkey) {
		Name = name;
		Hotkey = hotkey;
	}
}
