using UnityEngine;
using System.Collections.Generic;

// This class is the base for all personal AI.
// It represents the ability for a unit to think and act on its own without input from any player (human or computer AI)
[RequireComponent(typeof(Controllable))]
[AddComponentMenu("AI/Personal AI")]
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
	private List<GameObject> _visibleObjects = new List<GameObject>();
	protected List<GameObject> VisibleObjects
	{
		get {
			_visibleObjects.RemoveAll(m => m == null);
			return _visibleObjects;
		}
		set { _visibleObjects = value; }
	}
	
	protected Controllable Controllable { get; set; }
	
	protected virtual void Awake() {
		VisibleObjects = new List<GameObject>();
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
			VisibleObjects.Sort(CompareVisibleObjects);
			foreach(GameObject obj in VisibleObjects) {
				// Determine if other object is an enemy unit
				if(obj.CompareTag(GameUtil.TAG_UNIT) && Controllable.Owner.Relationships[obj.GetComponent<Controllable>().Owner] == PlayerRelationship.HOSTILE) {
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
	
	protected int CompareVisibleObjects(GameObject x, GameObject y) {
		return Vector3.Distance(x.transform.position, this.transform.position).CompareTo(Vector3.Distance(y.transform.position, this.transform.position));
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
		if(!VisibleObjects.Contains(obj)) {
			VisibleObjects.Add(obj);
		}
	}
	
	// Called when another object moves out of visual range
	public virtual void ObjectLeftVision(GameObject obj) {
		VisibleObjects.Remove(obj);
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
		//flee
		UpdateState();
	}
}
