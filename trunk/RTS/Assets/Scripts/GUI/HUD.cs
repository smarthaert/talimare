using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	
	public GUISkin skin;
	
	protected Player player;
	protected Rect resourceLevelsLocation;
	
	protected void Start() {
		player = Game.ThisPlayer;
		resourceLevelsLocation = new Rect(5, Screen.height-170, 100, 100);
	}
	
	protected void OnGUI() {
		GUI.skin = skin;
		// Use this if/when we need to scale the gui to other resolutions
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3(Screen.width / 1920.0, Screen.height / 1200.0, 1));
		
		RenderResourceLevels();
		RenderLowerPane();
	}
	
	protected void RenderResourceLevels() {
		int offset = 0;
		foreach(KeyValuePair<Resource, int> resourceLevel in player.PlayerStatus.resourceLevels) {
			Rect tempLocation = resourceLevelsLocation;
			tempLocation.y += offset;
			if(resourceLevel.Key == Resource.Food || resourceLevel.Key == Resource.Water || resourceLevel.Key == Resource.Power) {
				int resourceUpkeepMaximum = player.PlayerStatus.upkeepMaximums[resourceLevel.Key];
				int resourceAmountUsed = resourceUpkeepMaximum - resourceLevel.Value;
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceAmountUsed+" / "+resourceUpkeepMaximum.ToString());
			} else {
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceLevel.Value.ToString());
			}
			offset += 16;
		}
	}
	
	protected void RenderLowerPane() {
		GUI.BeginGroup(new Rect(0, Screen.height-Screen.height/4, Screen.width, Screen.height/4));
		RenderMapPane();
		RenderSelectedPane();
		RenderControlsPane();
		GUI.EndGroup();
	}
	
	protected void RenderMapPane() {
		GUI.Box(new Rect(0, 0, Screen.width/3, Screen.height/4), "");
	}
	
	protected void RenderSelectedPane() {
		GUI.Box(new Rect(Screen.width/3, 0, Screen.width/3, Screen.height/4), "");
		//TODO high: pull details of currently-selected Selectable from PlayerInput and display them... somehow
	}
	
	protected void RenderControlsPane() {
		GUI.Box(new Rect((Screen.width/3)*2, 0, Screen.width/3, Screen.height/4), "");
	}
}
