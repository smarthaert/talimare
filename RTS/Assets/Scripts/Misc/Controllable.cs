using UnityEngine;
using System.Collections.Generic;

// Defines the behavior of a Selectable which can be controlled, i.e. issued actions
public abstract class Controllable : Selectable {
	
	// This is the object's major reference point to its Player object, aka the object's owner
	public Player owner;
	
	// A list of Techs which apply to this object when they are gained
	public List<Tech> applicableTechs;
	
	// A queue to hold all current actions this object is tasked complete
	private Queue<Action> actionQueue = new Queue<Action>();
	
	protected override void Start() {
		base.Start();
		
		if(owner == null)
			Debug.Log("Player was never set for the Controllable: "+name+". It should be set immediately after instantiating the object.");
	}
	
	protected override void Update() {
		base.Update();
		
		ProcessActionQueue();
	}
	
	// Processes the action queue by starting the top action or removing it if it has completed
	private void ProcessActionQueue() {
		if(actionQueue.Count > 0) {
			Action topAction = actionQueue.Peek();
			if(!topAction.IsStarted) {
				topAction.Start();
			} else if(!topAction.IsRunning()) {
				actionQueue.Dequeue();
			}
		}
	}
	
	// Adds the given action to the action queue. If appendToQueue is false, the entire queue is aborted and the new action will be the only one remaining
	public void AddAction(Action action, bool appendToQueue) {
		if(!appendToQueue) {
			AbortActionQueue();
		}
		actionQueue.Enqueue(action);
	}
	
	// Aborts the entire action queue by aborting and removing each action
	public void AbortActionQueue() {
		while(actionQueue.Count > 0) {
			Action abortedAction = actionQueue.Dequeue();
			if(abortedAction.IsStarted) {
				abortedAction.Abort();
			}
		}
	}
	
	// Returns whether or not the multi-key is pressed (default shift, or the key that allows you to operate on multiple things at once)
	public bool IsMultiKeyPressed() {
		return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
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

