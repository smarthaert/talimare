using UnityEngine;
using System.Collections;

// Handles general player input unrelated to specific objects
public class PlayerInput : MonoBehaviour {
	
	// A visual marker for the current selection
	public GameObject selectionMarker;
	
	protected int allLayersMinusFogMask;
	
	protected SelectableControl currentSelection;
	protected GameObject currentMarker;
	
	void Start () {
		allLayersMinusFogMask = ~(1 << LayerMask.NameToLayer("FogOfWar"));
	}
	
	void Update () {
		// Send key pressed notification to the currently selected object
		if(currentSelection != null) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				DeselectCurrent();
			} else if(Input.anyKeyDown) {
				currentSelection.KeyPressed();
			}
		}
		
		// Handle mouse0 click (object selection)
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, allLayersMinusFogMask)) {
				// Note: this currently only works if the collider we hit is the same gameobject
				// in the hierarchy as has the Selectable script attached
				if(hit.collider.gameObject.GetComponent(typeof(SelectableControl)) != null) {
					if(hit.collider.gameObject.GetComponent(typeof(SelectableControl)) != currentSelection) {
						Select((SelectableControl)hit.collider.gameObject.GetComponent(typeof(SelectableControl)));
					}
				} else {
					DeselectCurrent();
				}
			} else {
				DeselectCurrent();
			}
		}
		
		// Handle mouse1 click (object action)
		if(Input.GetMouseButtonDown(1) && currentSelection != null) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, allLayersMinusFogMask)) {
				currentSelection.MouseAction(hit);
			}
		}
	}
	
	// Selects the given object, adding a visual marker
	void Select(SelectableControl selectable) {
		DeselectCurrent();
		currentSelection = selectable;
		currentMarker = (GameObject)Instantiate(selectionMarker, currentSelection.gameObject.transform.position, Quaternion.identity);
		currentMarker.transform.parent = currentSelection.gameObject.transform;
		currentSelection.Selected();
	}
	
	// Deselects the currently selected object
	void DeselectCurrent() {
		if(currentMarker != null) {
			Destroy(currentMarker);
		}
		currentMarker = null;
		
		if(currentSelection != null) {
			currentSelection.Deselected();
		}
		currentSelection = null;
	}
}
