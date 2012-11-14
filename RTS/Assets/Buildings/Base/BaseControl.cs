using UnityEngine;
using System.Collections;

public class BaseControl : BuildingControl {
	
	public KeyCode trainCivilianKey = KeyCode.C;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void KeyPressed() {
		if(Input.GetKeyDown(trainCivilianKey)) {
			Debug.Log("trainCivilianKey was pressed");
		}
	}
}
