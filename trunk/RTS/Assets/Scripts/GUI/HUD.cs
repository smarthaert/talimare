using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour {
	
	public GUISkin skin;
	
	protected Player Player { get; set; }
	protected Rect ResourceLevelsLocation { get; set; }
	protected PlayerInput PlayerInput { get; set; }
	protected Selectable CurrentSelection { get; set; }
	
	protected void Start() {
		Player = Game.ThisPlayer;
		PlayerInput = Game.PlayerInput;
		ResourceLevelsLocation = new Rect(5, Screen.height-170, 100, 100);
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
		foreach(KeyValuePair<Resource, int> resourceLevel in Player.PlayerStatus.resourceLevels) {
			Rect tempLocation = ResourceLevelsLocation;
			tempLocation.y += offset;
			if(resourceLevel.Key == Resource.Food) {
				int resourceUpkeepMaximum = Player.PlayerStatus.upkeepMaximums[resourceLevel.Key];
				int resourceAmountUsed = resourceUpkeepMaximum - resourceLevel.Value;
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceAmountUsed+" / "+resourceUpkeepMaximum.ToString());
			} else {
				GUI.Label(tempLocation, resourceLevel.Key.ToString()+": "+resourceLevel.Value.ToString());
			}
			offset += 16;
		}
	}
	
	protected void RenderLowerPane() {
		CurrentSelection = PlayerInput.CurrentSelection;
		
		GUI.BeginGroup(new Rect(0, Screen.height-Screen.height/4, Screen.width, Screen.height/4));
		RenderMapPane();
		RenderSelectedPane();
		RenderControlsPane();
		GUI.EndGroup();
	}
	
	protected void RenderMapPane() {
		//GUI.BeginGroup(new Rect(0, 0, Screen.width/3, Screen.height/4));
		GUILayout.BeginArea(new Rect(0, 0, Screen.width/3, Screen.height/4));
		
		GUI.Box(new Rect(0, 0, Screen.width/3, Screen.height/4), "");
		
		GUILayout.EndArea();
		//GUI.EndGroup();
	}
	
	protected void RenderSelectedPane() {
		//GUI.BeginGroup(new Rect(Screen.width/3, 0, Screen.width/3, Screen.height/4));
		GUILayout.BeginArea(new Rect(Screen.width/3, 0, Screen.width/3, Screen.height/4));
		
		GUI.Box(new Rect(0, 0, Screen.width/3, Screen.height/4), "");
		if(CurrentSelection != null) {
			GUILayout.Label(CurrentSelection.name);
			if(CurrentSelection.GetComponent<ControllableStatus>() != null) {
				RenderSelectedControllable();
			} else if(CurrentSelection.GetComponent<ResourceNode>() != null) {
				RenderSelectedResource();
			}
		}
		
		GUILayout.EndArea();
		//GUI.EndGroup();
	}
	
	protected void RenderSelectedControllable() {
		ControllableStatus status = CurrentSelection.GetComponent<ControllableStatus>();
		GUILayout.Label("HP: " + status.HP + " / " + status.maxHP);
		
		//check if unit
		if(CurrentSelection.CompareTag(GameUtil.TAG_UNIT)) {
			if(CurrentSelection.GetComponent<UnitStatus>() != null) {
				UnitStatus unitStatus = CurrentSelection.GetComponent<UnitStatus>();
				GUILayout.Label("Water: " + unitStatus.Water + " / " + unitStatus.maxWater);
			}
			
			//check if carrying resources
			if(CurrentSelection.GetComponent<MoveResourceTaskScript>() != null) {
				ResourceAmount heldResource = CurrentSelection.GetComponent<MoveResourceTaskScript>().HeldResource;
				if(heldResource != null) {
					GUILayout.Label("Carrying: " + heldResource.resource + " x " + heldResource.amount);
				}
			}
		}
		
		//check if building
		if(CurrentSelection.CompareTag(GameUtil.TAG_BUILDING)) {
			//check if water supplier
			if(CurrentSelection.GetComponent<WaterSupplier>() != null) {
				WaterSupplier waterSupplier = CurrentSelection.GetComponent<WaterSupplier>();
				GUILayout.Label("Water: " + waterSupplier.WaterHeld + " / " + waterSupplier.maxWaterHeld);
			}
			
			//check if power supplier
			if(CurrentSelection.GetComponent<PowerSupplier>() != null) {
				PowerSupplier powerSupplier = CurrentSelection.GetComponent<PowerSupplier>();
				GUILayout.Label("Power: " + (powerSupplier.powerSupplied-powerSupplier.FreePower) + " / " + powerSupplier.powerSupplied + " used");
			}
			
			if(CurrentSelection.GetComponent<BuildingStatus>() != null) {
				BuildingStatus buildingStatus = CurrentSelection.GetComponent<BuildingStatus>();
				if(buildingStatus.powerRequired > 0) {
					GUILayout.Label("Power is " + (buildingStatus.PowerEnabled ? "enabled" : "disabled") + " and" + (buildingStatus.Powered ? " " : " not ") + "supplied");
				}
			}
		}
		
		if(CurrentSelection.CompareTag(GameUtil.TAG_BUILD_PROGRESS)) {
			if(CurrentSelection.GetComponent<BuildProgressControl>() != null) {
				BuildProgressControl buildProgressControl = CurrentSelection.GetComponent<BuildProgressControl>();
				foreach(Resource resourse in buildProgressControl.StoredResources.Keys) {
					GUILayout.Label("Stored: " + resourse + " x " + buildProgressControl.StoredResources[resourse]);
				}
			}
		}
	}
	
	protected void RenderSelectedResource() {
		ResourceNode resource = CurrentSelection.GetComponent<ResourceNode>();
		GUILayout.Label(resource.CurrentAmount + " / " + resource.startingAmount);
	}
	
	protected void RenderControlsPane() {
		//GUI.BeginGroup(new Rect((Screen.width/3)*2, 0, Screen.width/3, Screen.height/4));
		GUILayout.BeginArea(new Rect((Screen.width/3)*2, 0, Screen.width/3, Screen.height/4));
		
		GUI.Box(new Rect(0, 0, Screen.width/3, Screen.height/4), "");
		if(CurrentSelection is Controllable && ((Controllable)CurrentSelection).Owner == Player) {
			RenderControls();
		}
		
		GUILayout.EndArea();
		//GUI.EndGroup();
	}
	
	protected void RenderControls() {
		Controllable controllable = (Controllable)CurrentSelection;
		foreach(ControlMenuItem controlMenuItem in controllable.CurrentControlMenu.MenuItems) {
			if(controlMenuItem.Enabled.Bool) {
				GUILayout.Label(controlMenuItem.Control.Name);
			} else {
				GUILayout.Label("-"+controlMenuItem.Control.Name+"-");
			}
		}
	}
}
