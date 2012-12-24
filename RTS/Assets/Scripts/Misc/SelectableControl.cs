using UnityEngine;
using System;

// Defines the behavior of a selectable GameObject
public class SelectableControl : MonoBehaviour {
	
	[NonSerialized]
	public int objectId;
	
	protected virtual void Start () {
		Game.RegisterSelectable(this);
	}
	
	protected virtual void Update () {}
	
	// Called when this GameObject has been selected
	public virtual void Selected() {
		//play some audio file or something
	}
	
	// Called when this GameObject has been deselected
	public virtual void Deselected() {}
	
	// Called when mouse action button is clicked on any object while this GameObject is selected
	public virtual void MouseAction(RaycastHit hit) {}
	
	// Called when any key is pressed while this GameObject is selected
	public virtual void KeyPressed() {}
}
