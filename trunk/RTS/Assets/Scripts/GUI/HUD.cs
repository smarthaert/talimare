using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	
	public Player player;
	public GUISkin skin;
	
	protected Rect resourceLevelsLocation;
	
	void Start() {
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
		foreach(KeyValuePair<Resource, int> resourceLevel in player.playerStatus.resourceLevels) {
			Rect tempLocation = resourceLevelsLocation;
			tempLocation.y += offset;
			if(resourceLevel.Key == Resource.Food || resourceLevel.Key == Resource.Water || resourceLevel.Key == Resource.Power) {
				int resourceUpkeepMaximum = player.playerStatus.upkeepMaximums[resourceLevel.Key];
				int resourceAmountUsed = resourceUpkeepMaximum - resourceLevel.Value;
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceAmountUsed+" / "+resourceUpkeepMaximum.ToString());
			} else {
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceLevel.Value.ToString());
			}
			offset += 16;
		}
	}
}
