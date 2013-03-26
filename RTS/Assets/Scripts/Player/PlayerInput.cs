using UnityEngine;
using System.Collections;

// Handles general player input unrelated to specific objects. Can be placed anywhere as long as it is in the scene
public class PlayerInput : MonoBehaviour {
	
	// A visual marker for the current selection
	public GameObject selectionMarker;
	
	protected int ClickLayerMask { get; set; }
	
	protected Selectable currentSelection;
	protected Vision currentSelectionVision;
	protected GameObject currentMarker;
	public bool DeselectDisabled { get; set; }
	
	void Start () {
		ClickLayerMask = ~((1 << LayerMask.NameToLayer("FogOfWar")) + (1 << LayerMask.NameToLayer("Ignore Raycast")));
	}
	
	void Update () {
		if(currentSelection != null) {
			if(currentSelectionVision != null && currentSelectionVision.IsHiddenByFog) {
				// Current selection is now hidden by fog
				DeselectCurrent();
			} else {
				// Send any key pressed notifications to the currently selected object
				if(!DeselectDisabled && Input.GetKeyDown(KeyCode.Escape)) {
					DeselectCurrent();
				} else if(Input.anyKeyDown && CurrentSelectionIsMyControllable()) {
					((Controllable)currentSelection).KeyPressed();
				}
				
				// Handle mouse1 click (object action)
				if(Input.GetMouseButtonDown(1) && CurrentSelectionIsMyControllable()) {
					// Make sure the current selection is owned by this player
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if(Physics.Raycast(ray, out hit, Mathf.Infinity, ClickLayerMask)) {
						((Controllable)currentSelection).MouseAction(hit);
					}
				}
			}
		}
		
		// Handle mouse0 click (object selection)
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, ClickLayerMask)) {
				// Note: this currently only works if the collider we hit is the same gameobject
				// in the hierarchy as has the Selectable script attached
				GameObject clickedObject = hit.collider.gameObject;
				Selectable selectable = clickedObject.GetComponent<Selectable>();
				Vision vision = clickedObject.GetComponentInChildren<Vision>();
				if(selectable != null && (vision == null || !vision.IsHiddenByFog)) {
					if(selectable != currentSelection) {
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
	void Select(Selectable selectable) {
		DeselectCurrent();
		currentSelection = selectable;
		currentMarker = (GameObject)Instantiate(selectionMarker, currentSelection.gameObject.transform.position, Quaternion.identity);
		currentMarker.transform.parent = currentSelection.gameObject.transform;
		
		currentSelection.Selected();
		currentSelectionVision = currentSelection.GetComponentInChildren<Vision>();
		
		DeselectDisabled = false;
	}
	
	// Deselects the currently selected object
	void DeselectCurrent() {
		if(currentMarker != null)
			Destroy(currentMarker);
		currentMarker = null;
		
		if(currentSelection != null)
			currentSelection.Deselected();
		currentSelection = null;
		currentSelectionVision = null;
		
		DeselectDisabled = false;
	}
	
	protected bool CurrentSelectionIsMyControllable() {
		return (currentSelection is Controllable && ((Controllable)currentSelection).owner == Game.ThisPlayer);
	}
}
