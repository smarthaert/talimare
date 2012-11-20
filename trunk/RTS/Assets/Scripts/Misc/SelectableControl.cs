using UnityEngine;
using System.Collections;

// Defines the behavior of a selectable GameObject
public class SelectableControl : MonoBehaviour {
	
	protected virtual void Start () {}
	
	protected virtual void Update () {}
	
	// Called when this GameObject has been selected
	public void Selected() {
		//play some audio file or something
	}
	
	// Called when this GameObject has been deselected
	public void Deselected() {}
	
	// Called when mouse action button is clicked on any object while this GameObject is selected
	public virtual void MouseAction(RaycastHit hit) {}
	
	// Called when any key is pressed while this GameObject is selected
	public virtual void KeyPressed() {}
}
