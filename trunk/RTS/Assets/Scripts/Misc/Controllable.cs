using UnityEngine;
using System.Collections.Generic;
using Wintellect.PowerCollections;

// Defines the behavior of a Selectable which can be controlled, i.e. issued actions
public abstract class Controllable : Selectable {
	
	// A list of Techs which apply to this object when they are gained
	public List<Tech> applicableTechs;
	
	// This is the object's major reference point to its Player object, aka the object's owner
	public Player Owner { get; protected set; }
	
	// A collection of ControlMenus which this Controllable has and can be displayed on the HUD, keyed by menu name
	protected Dictionary<string, ControlMenu> ControlMenus { get; set; }
	// The current ControlMenu which is selected and should be displayed on the HUD
	public ControlMenu CurrentControlMenu { get; set; }
	
	// A queue to hold all current tasks this object is tasked complete
	private Deque<Task> taskQueue = new Deque<Task>();
	
	public const string BASE_MENU_NAME = "baseMenu";
	
	protected override void Awake() {
		base.Awake();
		
		// Add a kinematic rigidbody in order to make collisions work
		gameObject.AddComponent<Rigidbody>().isKinematic = true;
		
		Owner = transform.parent.GetComponent<Player>();
	}
	
	protected override void Start() {
		base.Start();
		
		if(Owner == null) {
			Debug.LogError("Player was never set for the Controllable: "+name+". It should be set immediately after instantiating the object.");
		}
		
		ControlMenus = new Dictionary<string, ControlMenu>();
		BuildControlMenus();
	}
	
	protected abstract void BuildControlMenus();
	
	protected override void Update() {
		base.Update();
		
		ProcessTaskQueue();
	}
	
	public virtual void DisableCurrentMenuItems() {
		if(CurrentControlMenu != null) {
			foreach(ControlMenuItem menuItem in CurrentControlMenu.MenuItems) {
				if(menuItem.RequiresPower && GetComponent<BuildingStatus>() != null && !GetComponent<BuildingStatus>().Powered) {
					menuItem.Enabled = new BoolAndString(false, "Power is required for that.");
				} else if(menuItem.Creatable != null) {
					menuItem.Enabled = menuItem.Creatable.CanCreate(Owner);
				} else {
					menuItem.Enabled = new BoolAndString(true);
				}
			}
		}
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
	public virtual void MouseAction(RaycastHit hit) {}
	
	// Called when a ControlCode is received while this Controllable is selected
	public virtual void ReceiveControlCode(string controlCode) {
		// Handle menu navigation
		ControlMenuItem selectedMenuItem = CurrentControlMenu.GetMenuItemWithCode(controlCode);
		if(selectedMenuItem.DestinationMenu != null) {
			CurrentControlMenu = ControlMenus[selectedMenuItem.DestinationMenu];
			DisableCurrentMenuItems();
		}
	}
	
	public override void Deselected() {
		base.Deselected();
		
		CurrentControlMenu = ControlMenus[BASE_MENU_NAME];
	}
}

