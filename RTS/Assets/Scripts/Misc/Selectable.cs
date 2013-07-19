using UnityEngine;
using System;

// Defines the behavior of a selectable GameObject
[AddComponentMenu("Misc RTS/Selectable")]
public class Selectable : MonoBehaviour {
	
	protected virtual void Awake() {}
	
	protected virtual void Start() {}
	
	protected virtual void Update() {}
	
	// Called when this GameObject has been selected
	public virtual void Selected() {
		//play some audio file or something
	}
	
	// Called when this GameObject has been deselected
	public virtual void Deselected() {}
}