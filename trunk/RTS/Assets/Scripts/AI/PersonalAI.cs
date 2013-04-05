using UnityEngine;
using System.Collections.Generic;

// This class is the base for all personal AI.
// It represents the ability for a unit to think and act on its own without input from any player (human or computer AI)
[RequireComponent(typeof(Controllable))]
public class PersonalAI : MonoBehaviour {
	
	public AIState targetStateOnIdle = AIState.Idle;
	public AIStance stance = AIStance.Defensive;
	public bool combatOverridesWork = false;
	
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
		if(State == AIState.Idle) {
			HandleIdleState();
		}
	}
	
	// Sets the current AIState based on the object's current task
	protected virtual void UpdateState() {
		AIState savedState = State;
		if(Controllable.GetCurrentTask() == null) {
			State = AIState.Idle;
		} else if(Controllable.GetCurrentTask().TaskScript is AttackTaskScript) {
			State = AIState.Fighting;
		} else {
			State = AIState.Working;
		}
		if(State != savedState) {
			StateTimer = 0f;
		} else {
			StateTimer += Time.deltaTime;
		}
	}
	
	// Decides what actions to take on AIState.IDLE
	protected virtual void HandleIdleState() {
		if(targetStateOnIdle == AIState.Idle)
			ContinueIdleState();
		else if(targetStateOnIdle == AIState.Working)
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
		
		// Determine if other object is an enemy
		if(Controllable.owner.relationships[obj.GetComponent<Controllable>().owner] == PlayerRelationship.HOSTILE) {
			// Determine if object is in a state to respond
			if(State == AIState.Idle || (State == AIState.Working && combatOverridesWork)) {
				// Act based on object's stance
				if(stance == AIStance.Aggressive) {
					Fight(obj);
				} else if(stance == AIStance.Avoidive) {
					//TODO low: flee
				}
			}
		}
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		visibleObjects.Remove(obj);
	}
	
	// Called whenever this object takes damage
	public virtual void TookDamage(GameObject source) {
		// Determine if other object is an enemy
		if(Controllable.owner.relationships[source.GetComponent<Controllable>().owner] == PlayerRelationship.HOSTILE) {
			// Determine if object is in a state to respond
			if(State == AIState.Idle || (State == AIState.Working && combatOverridesWork)) {
				// Act based on object's stance
				if(stance == AIStance.Aggressive || stance == AIStance.Defensive) {
					Fight(source);
				} else if(stance == AIStance.Passive || stance == AIStance.Avoidive) {
					//TODO low: flee
				}
			}
		}
	}
	
	protected virtual void Fight(GameObject target) {
		Controllable.AddTaskInterrupt(new Task(GetComponent<AttackTaskScript>(), target));
	}
}
