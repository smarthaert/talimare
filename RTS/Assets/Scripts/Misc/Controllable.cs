using UnityEngine;
using System.Collections.Generic;
using Wintellect.PowerCollections;

// Defines the behavior of a Selectable which can be controlled, i.e. issued actions
public abstract class Controllable : Selectable {
	
	// This is the object's major reference point to its Player object, aka the object's owner
	public Player owner;
	
	// A list of Techs which apply to this object when they are gained
	public List<Tech> applicableTechs;
	
	// A set of KeyControlCodes which this Controllable uses and should be displayed on the HUD
	//TODO high: make this a multi-layer list so that hitting certain keys opens up a new set of keys (like for the build menu)
	public OrderedSet<KeyControlCode> keyControlCodes = new OrderedSet<KeyControlCode>();
	
	// A queue to hold all current tasks this object is tasked complete
	private Deque<Task> taskQueue = new Deque<Task>();
	
	protected override void Start() {
		base.Start();
		
		if(owner == null)
			Debug.Log("Player was never set for the Controllable: "+name+". It should be set immediately after instantiating the object.");
	}
	
	protected override void Update() {
		base.Update();
		
		ProcessTaskQueue();
	}
	
	// Processes the action queue by starting the top action or removing it if it has completed
	private void ProcessTaskQueue() {
		if(taskQueue.Count > 0) {
			Task frontTask = taskQueue.GetAtFront();
			if(!frontTask.IsStarted) {
				frontTask.Start();
			} else if(!frontTask.IsRunning()) {
				taskQueue.RemoveFromFront();
			}
		}
	}
	
	// Adds the given task to the task queue. If appendToQueue is false, the entire queue is aborted and the new task will be the only one remaining
	public void AddTask(Task task, bool appendToQueue) {
		if(!appendToQueue) {
			AbortTaskQueue();
		}
		taskQueue.AddToBack(task);
	}
	
	// Adds a task as an interrupt, which will execute immediately and any current task becomes paused
	public void AddTaskInterrupt(Task task) {
		if(taskQueue.Count > 0) {
			taskQueue.GetAtFront().Pause();
		}
		taskQueue.AddToFront(task);
	}
	
	// Aborts the entire task queue by aborting and removing each task
	public void AbortTaskQueue() {
		while(taskQueue.Count > 0) {
			Task abortedTask = taskQueue.RemoveFromFront();
			if(abortedTask.IsStarted) {
				abortedTask.Abort();
			}
		}
	}
	
	public Task GetCurrentTask() {
		if(taskQueue.Count > 0) {
			return taskQueue.GetAtFront();
		} else {
			return null;
		}
	}
	
	// Called when mouse action button is clicked on any object while this Controllable is selected
	public virtual void MouseAction(RaycastHit hit) {}
	
	// Called when any key is pressed while this Controllable is selected
	public virtual void KeyPressed() {}
}

