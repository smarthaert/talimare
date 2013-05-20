using UnityEngine;
using System.Collections.Generic;

// This class is the base for all personal AI.
// It represents the ability for a unit to think and act on its own without input from any player (human or computer AI)
[RequireComponent(typeof(Controllable))]
public class PersonalAI : MonoBehaviour {
	
	public AIState targetStateOnIdle = AIState.Idle;
	public AIStance stance = AIStance.Defensive;
	public bool combatOverridesWork = false;
	public bool autoAttackBuildings = false;
	
	public const float workCheckInterval = 2f;
	
	// The object's last state, used for tracking state changes
	public AIState LastState { get; set; }
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
		if(State != LastState) {
			StateTimer = 0f;
			LastState = State;
		} else {
			StateTimer += Time.deltaTime;
		}
		
		HandleObjectsInVision();
		
		if(State == AIState.Idle) {
			HandleIdleState();
		}
	}
	
	// Sets the current AIState based on the object's current task.
	// Should be called at the beginning of Update, and once after every action which may affect the object's state
	protected virtual void UpdateState() {
		if(Controllable.GetCurrentTask() == null) {
			State = AIState.Idle;
		} else if(Controllable.GetCurrentTask() is AttackTask) {
			State = AIState.Fighting;
		} else {
			State = AIState.Working;
		}
	}
	
	// Handles any actions which need to be taken toward objects in vision
	protected virtual void HandleObjectsInVision() {
		// Determine if object is in a state to respond to other objects
		if(State == AIState.Idle || (State == AIState.Working && combatOverridesWork)) {
			//TODO low: sort visibleObjects by distance before looping through
			foreach(GameObject obj in visibleObjects.ToArray()) {
				// Determine if other object is an enemy unit
				if(obj == null) {
					visibleObjects.Remove(obj);
				} else if(obj.CompareTag(GameUtil.TAG_UNIT) && Controllable.Owner.Relationships[obj.GetComponent<Controllable>().Owner] == PlayerRelationship.HOSTILE) {
					// Act based on object's stance
					if(stance == AIStance.Aggressive) {
						Fight(obj);
						break;
					} else if(stance == AIStance.Avoidive) {
						Flee(obj);
						break;
					}
				}
			}
		}
	}
	
	// Decides what action to take when state is AIState.IDLE
	protected virtual void HandleIdleState() {
		if(targetStateOnIdle == AIState.Idle) {
			ContinueIdling();
		} else if(targetStateOnIdle == AIState.Working) {
			if(StateTimer >= workCheckInterval) {
				CheckForWork();
				StateTimer = 0f;
			}
		}
	}
	
	// Decides what action to take to continue being in AIState.IDLE.
	// Default implementation is to just do nothing
	protected virtual void ContinueIdling() {}
	
	// Decides what action to take to get to AIState.WORKING.
	// Default implementation is to ask the strategic AI
	protected virtual void CheckForWork() {
		Controllable.Owner.StrategicAI.AssignJob(Controllable, true);
	}
	
	// Called when another object moves into visual range
	public virtual void ObjectEnteredVision(GameObject obj) {
		if(!visibleObjects.Contains(obj)) {
			visibleObjects.Add(obj);
		}
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		visibleObjects.Remove(obj);
	}
	
	// Called whenever this object takes damage
	public virtual void TookDamage(GameObject source) {
		// Determine if object is in a state to respond
		if(State == AIState.Idle || (State == AIState.Working && combatOverridesWork)) {
			// Determine if other object is an enemy
			if(Controllable.Owner.Relationships[source.GetComponent<Controllable>().Owner] == PlayerRelationship.HOSTILE) {
				// Act based on object's stance
				if(stance == AIStance.Aggressive || stance == AIStance.Defensive) {
					Fight(source);
				} else if(stance == AIStance.Passive || stance == AIStance.Avoidive) {
					Flee(source);
				}
			}
		}
	}
	
	protected virtual void Fight(GameObject target) {
		Controllable.AddTaskInterrupt(new AttackTask(GetComponent<AttackTaskScript>(), target));
		UpdateState();
	}
	
	protected virtual void Flee(GameObject fleeFrom) {
		//TODO low: flee
		UpdateState();
	}
}
