using UnityEngine;

public class KeyControl {
	public string Name { get; set; }
	public KeyCode Key { get; set; }
	
	public KeyControl(string name, KeyCode key) {
		Name = name;
		Key = key;
	}
}
