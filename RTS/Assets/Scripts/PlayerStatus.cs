using UnityEngine;
using System.Collections;

// Keeps information about a player's current status
public class PlayerStatus : MonoBehaviour {
	// Resources
	public int food = 0;
	public int water = 0;
	public int electricity = 0;
	public int copper = 0;
	public int iron = 0;
	public int coal = 0;
	public int steel = 0;
	public int oil = 0;
	public int aluminum = 0;
	public int uranium = 0;
	public int unobtanium = 0;
	
	// Currents
	private GameObject currentSelection;

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
				if(hit.collider.gameObject.GetComponent("Selectable") != null) {
					if(currentSelection != null)
						DeselectCurrent();
					currentSelection = hit.collider.gameObject;
					hit.collider.gameObject.GetComponent("Selectable").SendMessage("Select");
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
			currentSelection.SendMessage("Deselect");
		currentSelection = null;
	}
}
