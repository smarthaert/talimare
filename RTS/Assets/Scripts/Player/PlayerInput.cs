using UnityEngine;
using System.Collections.Generic;

// Handles general player input unrelated to specific objects. Can be placed anywhere as long as it is in the scene
[AddComponentMenu("Player/Player Input")]
public class PlayerInput : MonoBehaviour {
	
	// A visual marker for the current selection
	public GameObject selectionMarker;
	
	protected int ClickLayerMask { get; set; }
	
	public Selectable CurrentSelection { get; protected set; }
	protected Vision CurrentSelectionVision { get; set; }
	protected GameObject CurrentMarker { get; set; }
	
	protected void Awake() {
		ClickLayerMask = ~((1 << LayerMask.NameToLayer("FogOfWar")) + (1 << LayerMask.NameToLayer("Ignore Raycast")));
	}
	
	protected void Update() {
		if(CurrentSelection != null) {
			if((CurrentSelectionVision != null && CurrentSelectionVision.IsHiddenByFog)
					|| (Input.GetKeyDown(KeyCode.Escape) && (!CurrentSelectionIsMyControllable() || !MenuHasBackButton(((Controllable)CurrentSelection).CurrentControlMenu)))) {
				//selection is hidden by fog, or the escape key was pressed
				DeselectCurrent();
			} else if(CurrentSelectionIsMyControllable()) {
				Controllable currentControllable = (Controllable)CurrentSelection;
				
				currentControllable.DisableCurrentMenuItems();
				
				// Send any key pressed notifications to the currently selected object
				if(Input.anyKeyDown) {
					foreach(ControlMenuItem menuItem in GetMenuItemsSelectedByCurrentKeys(currentControllable.CurrentControlMenu)) {
						if(menuItem.Enabled.Bool) {
							currentControllable.SendMessage("ReceiveControlCode", menuItem.ControlCode, SendMessageOptions.DontRequireReceiver);
						} else {
							//print this out in the middle of the player's screen
							Debug.Log(menuItem.Enabled.String);
						}
					}
				}
				
				// Handle mouse1 click (object action)
				if(Input.GetMouseButtonDown(1)) {
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if(Physics.Raycast(ray, out hit, Mathf.Infinity, ClickLayerMask)) {
						currentControllable.SendMessage("ReceiveMouseAction", hit, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
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
	}
	
	// Selects the given object, adding a visual marker
	public void Select(Selectable selectable) {
		DeselectCurrent();
		CurrentSelection = selectable;
		CurrentMarker = (GameObject)Instantiate(selectionMarker, CurrentSelection.gameObject.transform.position, Quaternion.identity);
		CurrentMarker.transform.parent = CurrentSelection.gameObject.transform;
		
		CurrentSelection.Selected();
		CurrentSelectionVision = CurrentSelection.GetComponentInChildren<Vision>();
	}
	
	// Deselects the currently selected object
	protected void DeselectCurrent() {
		if(CurrentMarker != null)
			Destroy(CurrentMarker);
		CurrentMarker = null;
		
		if(CurrentSelection != null)
			CurrentSelection.Deselected();
		CurrentSelection = null;
		CurrentSelectionVision = null;
	}
	
	protected bool CurrentSelectionIsMyControllable() {
		return (CurrentSelection is Controllable && ((Controllable)CurrentSelection).Owner == Game.ThisPlayer);
	}
	
	// Returns whether or not the multi-key is pressed (default shift, or the key that allows you to operate on multiple things at once)
	public bool IsMultiKeyPressed() {
		return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
	}
	
	public List<ControlMenuItem> GetMenuItemsSelectedByCurrentKeys(ControlMenu menu) {
		List<ControlMenuItem> controlMenuItems = new List<ControlMenuItem>();
		foreach(ControlMenuItem item in menu.MenuItems) {
			if(Input.GetKeyDown(item.Control.Hotkey)) {
				controlMenuItems.Add(item);
			}
		}
		return controlMenuItems;
	}
	
	public bool MenuHasBackButton(ControlMenu menu) {
		foreach(ControlMenuItem item in menu.MenuItems) {
			if(item.ControlCode.Equals(ControlStore.MENU_BACK) || item.ControlCode.Equals(ControlStore.MENU_CANCEL)) {
				return true;
			}
		}
		return false;
	}
}
