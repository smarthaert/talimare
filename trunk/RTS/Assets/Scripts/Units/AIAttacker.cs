using UnityEngine;
using System.Collections;

// Handles unit attacking
[RequireComponent(typeof(AIPathfinder))]
public class AIAttacker : ActionScript {
	
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
	protected GameObject target;
	// An internal var to track who we're targeting for each individual attack sequence
	protected GameObject currentAttackTarget;
	
	protected AIPathfinder pathfinder;
	
	void Awake() {
		pathfinder = GetComponent<AIPathfinder>();
	}
	
	void Update() {
		if(currentAttackTarget == null) {
			// Not currently in an attack sequence (either due to being on cooldown, out of range, or have no target)
			if(attackCooldownTimer > 0)
				attackCooldownTimer -= Time.deltaTime;
			if(target != null) {
				// Have a target, so try to attack or move
				if(IsInRange(target.transform)) {
					pathfinder.StopMoving();
					// In range, start attacking if cooldown is finished
					if(attackCooldownTimer <= 0) {
						attackTimer = attackTime;
						currentAttackTarget = target;
					}
				} else {
					// Not in range, make sure we're moving into range
					pathfinder.Move(target.transform);
				}
			}
		} else {
			// Currently in an attack sequence
			attackTimer -= Time.deltaTime;
			if(attackTimer <= 0) {
				// Check attack range at end of attack sequence
				if(IsInRange(currentAttackTarget.transform)) {
					// Apply damage
					currentAttackTarget.GetComponent<UnitStatus>().Damage(attackDamage);
				}
				// Start cooldown timer
				attackCooldownTimer = attackCooldown;
				currentAttackTarget = null;
			}
		}
	}
	
	public override void StartAction(object target) {
		this.target = (GameObject)target;
	}
	
	public override bool IsActing() {
		return currentAttackTarget != null;
	}
	
	public override void StopAction() {
		target = null;
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
