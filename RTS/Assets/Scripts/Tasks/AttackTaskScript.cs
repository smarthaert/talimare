using UnityEngine;
using System.Collections;

// Handles unit attacking
[RequireComponent(typeof(Controllable))]
[RequireComponent(typeof(MoveTaskScript))]
[AddComponentMenu("Tasks/Attack")]
public class AttackTaskScript : MonoBehaviour {
	
	public float attackRange = 0;
	public int attackDamage = 0;
	public float attackTime = 0;
	public float attackCooldown = 0;
	
	protected float attackTimer = 0;
	protected float attackCooldownTimer = 0;
	
	// Whether or not the unit should get an attack range bonus due to higher ground
	public bool heightBonus = true;
	
	protected float heightBonusMinHeight = 3;
	protected float heightBonusPercentCap = 15;
	
	// The unit's attack target. If this is set, the unit should be actively trying to attack the target
	protected GameObject Target { get; set; }
	// An internal var to track who we're targeting for each individual attack sequence
	protected GameObject CurrentAttackTarget { get; set; }
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
	}
	
	public void ReceiveMouseAction(RaycastHit hit) {
		if(hit.transform.CompareTag(GameUtil.TAG_UNIT)) {
			Controllable targetControl = hit.transform.GetComponent<Controllable>();
			if(targetControl != null && Controllable.Owner != targetControl.Owner && Controllable.Owner.Relationships[targetControl.Owner] == PlayerRelationship.HOSTILE) {
				Controllable.AddTask(new AttackTask(GetComponent<AttackTaskScript>(), targetControl.gameObject), Game.PlayerInput.IsMultiKeyPressed());
			}
		}
	}
	
	protected void Update() {
		if(CurrentAttackTarget == null) {
			// Not currently in an attack sequence (either due to being on cooldown, out of range, or have no target)
			if(attackCooldownTimer > 0)
				attackCooldownTimer -= Time.deltaTime;
			if(Target != null) {
				// Have a target, so try to attack or move
				if(IsInRange(Target.transform)) {
					MoveTaskScript.StopTask();
					// In range, start attacking if cooldown is finished
					if(attackCooldownTimer <= 0) {
						attackTimer = attackTime;
						CurrentAttackTarget = Target;
					}
				} else {
					// Not in range, make sure we're moving into range
					MoveTaskScript.StartTask(Target.transform);
				}
			}
		} else {
			// Currently in an attack sequence
			attackTimer -= Time.deltaTime;
			if(attackTimer <= 0) {
				// Check attack range at end of attack sequence
				if(IsInRange(CurrentAttackTarget.transform)) {
					CombatRouter.DealDamage(this.gameObject, CurrentAttackTarget, attackDamage);
				}
				// Start cooldown timer
				attackCooldownTimer = attackCooldown;
				CurrentAttackTarget = null;
			}
		}
	}
	
	public void StartTask(GameObject target) {
		if(Target != target) {
			Target = target;
		}
	}
	
	public bool IsTaskRunning() {
		return Target != null;
	}
	
	public void StopTask() {
		Target = null;
		MoveTaskScript.StopTask();
	}
	
	protected bool IsInRange(Transform targetTransform) {
		float tempAttackRange = attackRange;
		if(heightBonus) {
			float heightDifference = (this.transform.position - targetTransform.position).y;
			if(heightDifference > heightBonusMinHeight) {
				tempAttackRange *= (1 + (heightDifference - heightBonusMinHeight) / heightDifference) * (heightBonusPercentCap / 100);
			}
		}
		return (targetTransform.position - this.transform.position).magnitude <= tempAttackRange;
	}
}
