using UnityEngine;
using System.Collections.Generic;

public class KeyControlStore {
	public static Dictionary<KeyControlCode, KeyControl> KeyControlMap { get; protected set; }
	
	static KeyControlStore() {
		KeyControlMap = new Dictionary<KeyControlCode, KeyControl>();
		
		KeyControlMap.Add(KeyControlCode.STOP, new KeyControl("Stop", KeyCode.S));
		
		KeyControlMap.Add(KeyControlCode.CIVILIAN, new KeyControl("Civilian", KeyCode.C));
	}
}