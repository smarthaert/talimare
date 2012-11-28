using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	
	public GUISkin skin;
	
	protected Rect resourceLevelsLocation;
	
	protected static PlayerStatus playerStatus;
	
	void Start() {
		if(playerStatus == null)
			playerStatus = GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
		
		resourceLevelsLocation = new Rect(5, Screen.height-170, 100, 100);
	}
	
	void OnGUI() {
		GUI.skin = skin;
		// Use this if/when we need to scale the gui to other resolutions
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3(Screen.width / 1920.0, Screen.height / 1200.0, 1));
		
		RenderResourceLevels();
	}
	
	void RenderResourceLevels() {
		int offset = 0;
		foreach(KeyValuePair<Resource, int> resourceLevel in playerStatus.resourceLevels) {
			Rect tempLocation = resourceLevelsLocation;
			tempLocation.y += offset;
			GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceLevel.Value.ToString());
			offset += 16;
		}
	}
}
