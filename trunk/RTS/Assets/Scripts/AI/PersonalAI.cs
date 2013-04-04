using UnityEngine;
using System.Collections.Generic;

// This class is the base for all personal AI.
// It represents the ability for a unit to think and act on its own without input from any player (human or computer AI)
public class PersonalAI : MonoBehaviour {
	
	public AIStance stance = AIStance.Defensive;
	public AIState targetStateOnIdle = AIState.IDLE;
	
	public AIState State { get; set; }
	// Tracks how long the AI has been in its current state
	protected float StateTimer { get; set; }
	
	// Tracks all objects which are currently visible and owned by another player
	protected List<GameObject> visibleObjects = new List<GameObject>();
	
	protected Controllable Controllable { get; set; }
	
	protected virtual void Awake() {
		Controllable = GetComponent<Controllable>();
	}
	
	protected virtual void Start() {}
	
	protected virtual void Update() {
		UpdateState();
		if(State == AIState.IDLE)
			HandleIdleState();
	}
	
	// Sets the current AIState based on the object's current task
	protected virtual void UpdateState() {
		AIState savedState = State;
		if(Controllable.GetCurrentTask().TaskScript == null) {
			State = AIState.IDLE;
		} else if(Controllable.GetCurrentTask().TaskScript is AttackTaskScript) {
			State = AIState.FIGHTING;
		} else {
			State = AIState.WORKING;
		}
		if(State != savedState) {
			StateTimer = 0f;
		} else {
			StateTimer += Time.deltaTime;
		}
	}
	
	// Decides what actions to take on AIState.IDLE
	protected virtual void HandleIdleState() {
		if(targetStateOnIdle == AIState.IDLE)
			ContinueIdleState();
		else if(targetStateOnIdle == AIState.WORKING)
			ForceWorkingState();
	}
	
	// Decides what actions to take to continue being in AIState.IDLE.
	// Default implementation is to just do nothing.
	protected virtual void ContinueIdleState() {}
	
	// Decides what actions to take to get to AIState.WORKING.
	// Intended to be overridden if needed, this method would be responsible for deciding what task to take to stay busy
	protected virtual void ForceWorkingState() {}
	
	// Called when another object moves into visual range
	public virtual void ObjectEnteredVision(GameObject obj) {
		visibleObjects.Add(obj);
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		visibleObjects.Remove(obj);
	}
}
