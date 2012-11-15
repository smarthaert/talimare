using UnityEngine;
using System.Collections;

// Defines the behavior of a selectable GameObject
// BuildingControl and UnitControl extend this, but simpler objects can just use this component
public class Selectable : MonoBehaviour {
	
	// Use this for initialization
	protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}
	
	// Called when this GameObject has been selected
	public void Select() {
		//play some audio file or something
	}
	
	// Called when this GameObject has been deselected
	public void Deselect() {}
	
	// Called when this GameObject is selected and a key is pressed
	public virtual void KeyPressed() {}
}
