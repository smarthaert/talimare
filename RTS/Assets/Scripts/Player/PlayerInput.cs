using UnityEngine;
using System.Collections.Generic;

// Handles general player input unrelated to specific objects. Should be placed 
[AddComponentMenu("Player/Player Input")]
public class PlayerInput : MonoBehaviour {
	
	// Various layer masks
	protected int ClickLayerMask { get; set; }
	protected int TerrainLayerMask { get; set; }
	
	// A visual marker for the current selection
	public GameObject selectionMarker;
	
	// Hold the currently-selected object and cache some associated things
	public Selectable CurrentSelection { get; protected set; }
	protected Vision CurrentSelectionVision { get; set; }
	protected GameObject CurrentMarker { get; set; }
	
	// Buildings which can be built
	public List<CreatableBuilding> buildings; //TODO med: build a list of buildable buildings on the fly
	
	// The currently-queued building to build
	protected BuildProgressControl QueuedBuildTarget { get; set; }
	
	// A collection of ControlMenus which can be displayed on the HUD, keyed by menu name
	protected Dictionary<string, ControlMenu> ControlMenus { get; set; }
	
	// The current ControlMenu which is active and should be displayed on the HUD
	private ControlMenu _currentControlMenu;
	public ControlMenu CurrentControlMenu {
		get { return _currentControlMenu; }
		set {
			if(value == null)
				_currentControlMenu = ControlMenus[ControlStore.MENU_BASE];
			else
				_currentControlMenu = value;
		}
	}
	
	protected void Awake() {
		ControlMenus = new Dictionary<string, ControlMenu>();
		ClickLayerMask = ~((1 << LayerMask.NameToLayer("FogOfWar")) + (1 << LayerMask.NameToLayer("Ignore Raycast")));
		TerrainLayerMask = 1 << LayerMask.NameToLayer("Walkable");
	}
	
	protected void Start() {
		BuildControlMenus();
	}
	
	protected void BuildControlMenus() {
		ControlMenu basePlayerMenu = new ControlMenu();
		basePlayerMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BUILDINGS, ControlStore.MENU_BUILDINGS));
		ControlMenus.Add(ControlStore.MENU_BASE, basePlayerMenu);
		
		ControlMenu createBuildingMenu = new ControlMenu();
		foreach(CreatableBuilding building in buildings) {
			createBuildingMenu.MenuItems.Add(new ControlMenuItem(building, ControlStore.MENU_CANCEL));
		}
		createBuildingMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_BACK, ControlStore.MENU_BASE));
		ControlMenus.Add(ControlStore.MENU_BUILDINGS, createBuildingMenu);
		
		ControlMenu cancelCreateMenu = new ControlMenu();
		cancelCreateMenu.MenuItems.Add(new ControlMenuItem(ControlStore.MENU_CANCEL, ControlStore.MENU_BUILDINGS));
		ControlMenus.Add(ControlStore.MENU_CANCEL, cancelCreateMenu);
		
		CurrentControlMenu = ControlMenus[ControlStore.MENU_BASE];
	}
	
	protected void Update() {
		DisableCurrentMenuItems();
		
		if(QueuedBuildTarget != null) {
			DrawQueuedBuildingAtMouse();
		}
		
		// If selection is hidden by fog, or the escape key was pressed
		if((CurrentSelectionVision != null && CurrentSelectionVision.IsHiddenByFog)
				|| (Input.GetKeyDown(KeyCode.Escape) && (!CurrentSelectionIsMyControllable() || !CurrentMenuHasBackButton()))) {
			DeselectCurrent();
		}
		
		// Handle mouse0 click (object selection)
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, ClickLayerMask)) {
				// Note: this only works if the collider we hit is the same gameobject in the hierarchy
				// as has the Selectable script attached, or if the main parent gameobject has a rigidbody attached
				// (raycast hits 'bubble up' to the first found rigidbody)
				GameObject clickedObject = hit.transform.gameObject;
				Selectable selectable = clickedObject.GetComponent<Selectable>();
				Vision vision = clickedObject.GetComponentInChildren<Vision>();
				if(selectable != null && (vision == null || !vision.IsHiddenByFog)) {
					if(selectable != CurrentSelection) {
						Select(selectable);
					}
				} else {
					DeselectCurrent();
				}
			} else {
				DeselectCurrent();
			}
		}
		
		// Handle any pressed keys
		if(Input.anyKeyDown) {
			foreach(ControlMenuItem menuItem in GetCurrentMenuItemsSelectedByCurrentKeys()) {
				if(menuItem.Enabled.Bool) {
					// Send control code to this input or any selected controllables
					if(CurrentSelectionIsMyControllable()) {
						((Controllable)CurrentSelection).SendMessage("ReceiveControlCode", menuItem.ControlCode, SendMessageOptions.DontRequireReceiver);
					} else {
						ReceiveControlCode(menuItem.ControlCode);
					}
					// Handle menu navigation if needed
					if(menuItem.DestinationMenu != null) {
						if(CurrentSelectionIsMyControllable()) {
							CurrentControlMenu = ((Controllable)CurrentSelection).ControlMenus[menuItem.DestinationMenu];
						} else {
							CurrentControlMenu = ControlMenus[menuItem.DestinationMenu];
						}
						DisableCurrentMenuItems();
					}
				} else {
					//print this out in the middle of the player's screen
					Debug.Log(menuItem.Enabled.String);
				}
			}
			if(Input.GetKeyDown(KeyCode.Delete) && CurrentSelectionIsMyControllable()) {
				((Controllable)CurrentSelection).SendMessage("ReceiveControlCode", ControlStore.DESTROY, SendMessageOptions.DontRequireReceiver);
			}
		}
		
		// Handle mouse1 click (object action)
		if(Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, ClickLayerMask)) {
				if(QueuedBuildTarget == null && CurrentSelectionIsMyControllable()) {
					((Controllable)CurrentSelection).SendMessage("ReceiveMouseAction", hit, SendMessageOptions.DontRequireReceiver);
				} else {
					ReceiveMouseAction(hit);
				}
			}
		}
	}
	
	// Selects the given object, adding a visual marker
	public void Select(Selectable selectable) {
		DeselectCurrent();
		CurrentSelection = selectable;
		CurrentMarker = (GameObject)Instantiate(selectionMarker, CurrentSelection.gameObject.transform.position, Quaternion.identity);
		CurrentMarker.transform.parent = CurrentSelection.gameObject.transform;
		
		CurrentSelection.Selected();
		CurrentSelectionVision = CurrentSelection.GetComponentInChildren<Vision>();
		
		if(CurrentSelectionIsMyControllable()) {
			CurrentControlMenu = ((Controllable)CurrentSelection).ControlMenus[ControlStore.MENU_BASE];
		}
	}
	
	// Deselects the currently selected object
	public void DeselectCurrent() {
		if(CurrentMarker != null)
			Destroy(CurrentMarker);
		CurrentMarker = null;
		
		if(CurrentSelection != null)
			CurrentSelection.Deselected();
		CurrentSelection = null;
		CurrentSelectionVision = null;
		
		CurrentControlMenu = ControlMenus[ControlStore.MENU_BASE];
	}
	
	public bool CurrentSelectionIsMyControllable() {
		return (CurrentSelection != null && CurrentSelection is Controllable && ((Controllable)CurrentSelection).Owner == Game.ThisPlayer);
	}
	
	// Returns whether or not the multi-key is pressed (default shift, or the key that allows you to operate on multiple things at once)
	public bool IsMultiKeyPressed() {
		return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
	}
	
	public virtual void DisableCurrentMenuItems() {
		if(CurrentControlMenu != null) {
			foreach(ControlMenuItem menuItem in CurrentControlMenu.MenuItems) {
				if(menuItem.RequiresPower && GetComponent<BuildingStatus>() != null && !GetComponent<BuildingStatus>().Powered) {
					menuItem.Enabled = new BoolAndString(false, "Power is required for that.");
				} else if(menuItem.Creatable != null) {
					menuItem.Enabled = menuItem.Creatable.CanCreate(Game.ThisPlayer);
				} else {
					menuItem.Enabled = new BoolAndString(true);
				}
			}
		}
	}
	
	public List<ControlMenuItem> GetCurrentMenuItemsSelectedByCurrentKeys() {
		List<ControlMenuItem> controlMenuItems = new List<ControlMenuItem>();
		foreach(ControlMenuItem item in CurrentControlMenu.MenuItems) {
			if(Input.GetKeyDown(item.Control.Hotkey)) {
				controlMenuItems.Add(item);
			}
		}
		return controlMenuItems;
	}
	
	public bool CurrentMenuHasBackButton() {
		foreach(ControlMenuItem item in CurrentControlMenu.MenuItems) {
			if(item.ControlCode.Equals(ControlStore.MENU_BACK) || item.ControlCode.Equals(ControlStore.MENU_CANCEL)) {
				return true;
			}
		}
		return false;
	}
	
	public void ReceiveMouseAction(RaycastHit hit) {
		if(QueuedBuildTarget != null) {
			CommitQueuedBuilding();
		}
	}
	
	public void ReceiveControlCode(string controlCode) {
		if(controlCode.Equals(ControlStore.MENU_CANCEL)) {
			RemoveQueuedBuildTarget(true);
		} else {
			// See if control code exists in buildings and if so, queue the BuildProgress object for that building
			foreach(CreatableBuilding building in buildings) {
				if(building.ControlCode.Equals(controlCode) && building.CanCreate(Game.ThisPlayer).Bool) {
					InstantiateBuildProgress(building);
				}
			}
		}
	}
	
	public void InstantiateBuildProgress(CreatableBuilding building) {
		QueuedBuildTarget = (GameUtil.InstantiateControllable(building.buildProgressControl, Game.ThisPlayer, Vector3.zero)).GetComponent<BuildProgressControl>();
		QueuedBuildTarget.name = building.gameObject.name+" (in progress)";
	}
	
	// Moves the queued building to where the mouse hits the ground
	protected void DrawQueuedBuildingAtMouse() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainLayerMask)) {
			// Shift the target position up on top of the ground and snap it to grid
			Vector3 position = hit.point;
			position.y += (QueuedBuildTarget.renderer.bounds.size.y / 2);
			position.x = Mathf.RoundToInt(position.x);
			position.y = Mathf.RoundToInt(position.y);
			position.z = Mathf.RoundToInt(position.z);
			QueuedBuildTarget.transform.position = position;
		}
	}
	
	// Commits the currently queued building at its current position and immediately tasks any selected unit on its BuildJob
	protected void CommitQueuedBuilding() {
		if(QueuedBuildTarget.FinishedBuildingCreatable.CanCreate(Game.ThisPlayer).Bool) {
			QueuedBuildTarget.Commit();
			if(CurrentSelectionIsMyControllable()) {
				QueuedBuildTarget.BuildJob.AssignNextJob((Controllable)CurrentSelection, Game.PlayerInput.IsMultiKeyPressed());
			}
		}
		if(Game.PlayerInput.IsMultiKeyPressed() && QueuedBuildTarget.FinishedBuildingCreatable.CanCreate(Game.ThisPlayer).Bool) {
			InstantiateBuildProgress(QueuedBuildTarget.FinishedBuildingCreatable);
		} else {
			RemoveQueuedBuildTarget(false);
		}
	}
	
	// Removes the currently queued build target reference. If true is passed, the target will be completely destroyed
	public void RemoveQueuedBuildTarget(bool andDestroyIt) {
		if(andDestroyIt) {
			Destroy(QueuedBuildTarget.gameObject);
		} else {
			QueuedBuildTarget = null;
			CurrentControlMenu = ControlMenus[ControlStore.MENU_BASE];
		}
	}
}
