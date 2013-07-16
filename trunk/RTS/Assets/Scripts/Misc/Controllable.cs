using UnityEngine;
using System.Collections.Generic;
using Wintellect.PowerCollections;

// Defines the behavior of a Selectable which can be controlled, i.e. issued actions
[RequireComponent(typeof(ControllableStatus))]
public abstract class Controllable : Selectable {
	
	// A list of Techs which apply to this object when they are gained
	public List<Tech> applicableTechs;
	
	// This is the object's major reference point to its Player object, aka the object's owner
	public Player Owner { get; set; }
	
	// A collection of ControlMenus which this Controllable has and can be displayed on the HUD, keyed by menu name
	public Dictionary<string, ControlMenu> ControlMenus { get; protected set; }
	
	// A queue to hold all current tasks this object is tasked complete
	private Deque<Task> taskQueue = new Deque<Task>();
	
	protected override void Awake() {
		base.Awake();
		
		if(transform.parent != null) {
			Owner = transform.parent.GetComponent<Player>();
		}
		
		// Add a kinematic rigidbody in order to make collisions work
		gameObject.AddComponent<Rigidbody>().isKinematic = true;
		
		ControlMenus = new Dictionary<string, ControlMenu>();
		BuildBaseControlMenu();
	}
	
	protected void BuildBaseControlMenu() {
		ControlMenus.Add(ControlStore.MENU_BASE, new ControlMenu());
	}
	
	protected override void Start() {
		base.Start();
		
		if(Owner == null) {
			Debug.LogError("Player was never set for the Controllable: "+name+". It should be set immediately after instantiating the object.");
		}
		
		SendMessage("BuildControlMenus", SendMessageOptions.DontRequireReceiver);
	}
	
	protected override void Update() {
		base.Update();
		
		ProcessTaskQueue();
	}
	
	// Processes the action queue by starting the top action or removing it if it has completed
	private void ProcessTaskQueue() {
		if(taskQueue.Count > 0) {
			Task frontTask = taskQueue.GetAtFront();
			if(!frontTask.Started) {
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
			if(taskQueue.GetAtFront().Started) {
				taskQueue.GetAtFront().Pause();
			}
		}
		taskQueue.AddToFront(task);
	}
	
	// Adds a task as an interrupt, but after the current task
	public void AddTaskInterruptAfterCurrent(Task task) {
		Task savedFront = taskQueue.RemoveFromFront();
		taskQueue.AddToFront(task);
		taskQueue.AddToFront(savedFront);
	}
	
	// Aborts the entire task queue by aborting and removing each task
	public void AbortTaskQueue() {
		while(taskQueue.Count > 0) {
			Task abortedTask = taskQueue.RemoveFromFront();
			if(abortedTask.Started) {
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
	public virtual void ReceiveMouseAction(RaycastHit hit) {}
	
	// Called when a ControlCode is received while this Controllable is selected
	public virtual void ReceiveControlCode(string controlCode) {
		if(controlCode.Equals(ControlStore.DESTROY)) {
			GetComponent<ControllableStatus>().Die();
		}
	}
	
	public override void Deselected() {
		base.Deselected();
	}
}

