using UnityEngine;
using System.Collections;

// Handles general player input unrelated to specific objects
public class PlayerInput : MonoBehaviour {
	
	private Selectable currentSelection;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Handle mouse0 click (object selection)
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit)) {
				if(hit.collider.gameObject.GetComponent(typeof(Selectable)) != null) {
					if(hit.collider.gameObject.GetComponent(typeof(Selectable)) != currentSelection) {
						if(currentSelection != null) {
							DeselectCurrent();
						}
						currentSelection = (Selectable)hit.collider.gameObject.GetComponent(typeof(Selectable));
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
		if(currentSelection != null)
			currentSelection.Deselect();
		currentSelection = null;
	}
}
