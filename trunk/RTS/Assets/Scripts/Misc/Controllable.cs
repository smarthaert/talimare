using UnityEngine;
using System.Collections.Generic;

// Defines the behavior of a Selectable which can be controlled, i.e. issued actions. Is assumed to have or provide vision
public abstract class Controllable : Selectable {
	
	// This is the object's major reference point to its Player object, aka the object's owner
	public Player owner;
	
	// A queue to hold all current actions this object is tasked complete
	public Queue<Action> actionQueue = new Queue<Action>();
	
	protected override void Start() {
		base.Start();
		
		if(owner == null)
			Debug.Log("Player was never set for the Controllable: "+name+". It should be set immediately after instantiating the object.");
	}
	
	protected override void Update() {
		base.Update();
		
		ProcessActionQueue();
	}
	
	protected void ProcessActionQueue() {
		if(actionQueue.Count > 0) {
			Action topAction = actionQueue.Peek();
			if(!topAction.IsStarted) {
				topAction.Start();
			}
			topAction.Update();
			if(!topAction.IsStarted) {
				actionQueue.Dequeue();
			}
		}
	}
	
	public void AbortActionQueue() {
		while(actionQueue.Count > 0) {
			Action abortedAction = actionQueue.Dequeue();
			if(abortedAction.IsStarted) {
				abortedAction.Abort();
			}
		}
	}
	
	// Called when mouse action button is clicked on any object while this Controllable is selected
	public virtual void MouseAction(RaycastHit hit) {}
	
	// Called when any key is pressed while this Controllable is selected
	public virtual void KeyPressed() {}
	
	// Called when another object moves into visual range
	public virtual void ObjectEnteredVision(GameObject obj) {}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {}
}

