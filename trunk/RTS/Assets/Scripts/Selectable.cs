using UnityEngine;
using System.Collections;

// Defines the behavior of a selectable GameObject
public class Selectable : MonoBehaviour {
	public GameObject selectionMarker;
	
	private GameObject currentMarker;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void Select() {
		currentMarker = (GameObject)Instantiate(selectionMarker, this.gameObject.transform.position, Quaternion.identity);
		currentMarker.transform.parent = this.gameObject.transform;
	}
	
	void Deselect() {
		Destroy(currentMarker);
		currentMarker = null;
	}
}
