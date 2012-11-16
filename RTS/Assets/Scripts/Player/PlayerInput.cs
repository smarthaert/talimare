using UnityEngine;
using System.Collections;

// Handles general player input unrelated to specific objects
public class PlayerInput : MonoBehaviour {
	
	// A visual marker for the current selection
	public GameObject selectionMarker;
	
	private SelectableControl currentSelection;
	private GameObject currentMarker;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Send key press notification to the currently selected object
		if(currentSelection != null && Input.anyKeyDown) {
			currentSelection.KeyPressed();
		}
		
		// Handle mouse0 click (object selection)
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit)) {
				// Note: this currently only works if the collider we hit is the same gameobject
				// in the hierarchy as has the Selectable script attached
				if(hit.collider.gameObject.GetComponent(typeof(SelectableControl)) != null) {
					if(hit.collider.gameObject.GetComponent(typeof(SelectableControl)) != currentSelection) {
						if(currentSelection != null) {
							DeselectCurrent();
						}
						// Select the clicked object, adding a visual marker
						currentSelection = (SelectableControl)hit.collider.gameObject.GetComponent(typeof(SelectableControl));
						currentMarker = (GameObject)Instantiate(selectionMarker, currentSelection.gameObject.transform.position, Quaternion.identity);
						currentMarker.transform.parent = currentSelection.gameObject.transform;
						currentSelection.Select();
					}
				} else {
					DeselectCurrent();
				}
			} else {
				DeselectCurrent();
			}
		}
	}
	
	void DeselectCurrent() {
		Destroy(currentMarker);
		currentMarker = null;
		if(currentSelection != null)
			currentSelection.Deselect();
		currentSelection = null;
	}
}
