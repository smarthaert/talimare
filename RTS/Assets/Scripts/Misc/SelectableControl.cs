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
	
	// Called when this GameObject is selected and a key is pressed
	public virtual void KeyPressed() {}
}
