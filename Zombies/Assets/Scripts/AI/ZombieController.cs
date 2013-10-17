using UnityEngine;
using RAIN.Core;
using FPSControl;
using RAIN.Motion;
using RAIN.Animation;
using RAIN.Path;

public class ZombieController : MonoBehaviour {
	
	public float meleeDamage;
	
	protected RAINAgent ai;
	protected DataController dataController; //health
	protected MoveLookTarget lastTargetPosition;
	
	public const string DEATH_ANIM = "DieEasy";

	protected void Start() {
		ai = GetComponent<RAINAgent>();
		dataController = GetComponent<DataController>();
		lastTargetPosition = new MoveLookTarget();
		lastTargetPosition.TargetType = MoveLookTarget.MoveLookTargetType.VECTOR;
		
		ai.Agent.actionContext.SetContextItem<bool>(Constants.AI_GOT_HIT_VAR, true);
		ai.Agent.actionContext.SetContextItem<bool>(Constants.AI_DEAD_VAR, false);
	}
	
	protected void Update() {
		ai.Agent.actionContext.SetContextItem<float>(Constants.AI_HEALTH_VAR, dataController.current);
		
		// Remember the target's last position in case he is no longer detected
		if(ai.Agent.actionContext.GetContextItem<GameObject>(Constants.AI_PLAYER_TARGET_VAR) != null) {
			lastTargetPosition.VectorTarget = ai.Agent.actionContext.GetContextItem<GameObject>(Constants.AI_PLAYER_TARGET_VAR).transform.position;
			ai.Agent.actionContext.SetContextItem<MoveLookTarget>(Constants.AI_LAST_TARGET_POS_VAR, lastTargetPosition);
		}
		
		if(dataController.current <= 0) {
			Die();
		}
	}
	
	public void ApplyDamage(DamageSource damageSource) {
		if(damageSource.sourceObjectType != DamageSource.DamageSourceObjectType.Obstacle) {
			ai.Agent.actionContext.SetContextItem<bool>(Constants.AI_GOT_HIT_VAR, true);
			
			MoveLookTarget moveLookTarget = new MoveLookTarget();
			moveLookTarget.TargetType = MoveLookTarget.MoveLookTargetType.VECTOR;
			moveLookTarget.VectorTarget = damageSource.fromPosition;
			ai.Agent.actionContext.SetContextItem<MoveLookTarget>(Constants.AI_LAST_HIT_POS_VAR, moveLookTarget);
			
			ai.Agent.actionContext.SetContextItem<GameObject>(Constants.AI_LAST_HIT_SOURCE_VAR, damageSource.sourceObject);
		}
		dataController.current -= damageSource.damageAmount;
	}
	
	public void MeleeAttack() {
		GameObject target = ai.Agent.actionContext.GetContextItem<GameObject>(Constants.AI_MELEE_TARGET_VAR);
		
		DamageSource damageSource = new DamageSource();
		damageSource.sourceObject = ai.Agent.Avatar.gameObject;
		damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.AI;
		damageSource.sourceType = DamageSource.DamageSourceType.MeleeAttack;
		damageSource.damageAmount = meleeDamage;
		damageSource.fromPosition = ai.Agent.Avatar.gameObject.transform.position;
		damageSource.appliedToPosition = target.transform.position;
		
		target.SendMessageUpwards("ApplyDamage", damageSource);
	}
	
	protected void Die() {
		ai.Agent.actionContext.SetContextItem<bool>(Constants.AI_DEAD_VAR, true);
		
		AnimationParams anim = new AnimationParams(DEATH_ANIM, AnimationPlayRequest.START, 1, 0, WrapMode.Once, 0);
		GetComponent<AIKinematicController>().HandleAIAnimationStateTransition(anim);
	}
}
