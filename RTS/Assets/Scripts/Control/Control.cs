using UnityEngine;

public class Control {
	public string Name { get; set; }
	public KeyCode Key { get; set; }
	
	public Control(string name, KeyCode key) {
		Name = name;
		Key = key;
	}
}
