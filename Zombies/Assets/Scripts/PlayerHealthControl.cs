using UnityEngine;
using FPSControl;
using RAIN.Ontology;

[RequireComponent(typeof(DataController))]
public class PlayerHealthControl : MonoBehaviour {
	
	public bool takeCollideDamage = true;
	public float collideDamageThreshold = 7;
	public string deathMusic = "PlayerDeath"; //this will move later
	public GUIText deathText; //this will move later
	
	public bool IsAlive { get; protected set; }
	
	protected DataController dataController;
	
	protected void Start() {
		IsAlive = true;
		dataController = GetComponent<DataController>();
	}
	
	// Hit by something
	protected void OnCollisionEnter(Collision collision) {
		float damage = collision.relativeVelocity.magnitude;
		
		// Take collider damage? (fall damage, etc)
		if(takeCollideDamage && damage > collideDamageThreshold) {
            DamageSource damageSource = new DamageSource();
            damageSource.damageAmount = damage - collideDamageThreshold;
            damageSource.fromPosition = collision.transform.position;
            damageSource.appliedToPosition = collision.transform.position;
            damageSource.sourceObject = collision.gameObject;
            damageSource.sourceObjectType = DamageSource.DamageSourceObjectType.Obstacle;
            damageSource.sourceType = DamageSource.DamageSourceType.StaticCollision;
			ApplyDamage(damageSource);
		}
	}
	
	public void SetHealth(float value) {
	    dataController.current = value;
	    DeathCheck();
	}
	
	public void ApplyHealth(float value) {
	    dataController.current += value;
	    DeathCheck();
	}
	
	public void ApplyDamage(DamageSource damageSource) {
		// Find quadrant of Player that was hit and broadcast that info
		Vector3 hitDir = damageSource.fromPosition - transform.position;
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		Vector3 left = transform.TransformDirection(Vector3.left);
		
		hitDir.y = 0f; //y distance should not be considered
		float forwardDist = Vector3.Dot(forward, hitDir);
		float sideDist = Vector3.Dot(left, hitDir);
		
		HurtQuadrant hurtQuad;
		if(Mathf.Abs(forwardDist) > Mathf.Abs(sideDist)) {
			if(forwardDist >= 0) {
				hurtQuad = HurtQuadrant.FRONT;
			} else {
				hurtQuad = HurtQuadrant.BACK;
			}
		} else {
			if(sideDist >= 0) {
				hurtQuad = HurtQuadrant.LEFT;
			} else {
				hurtQuad = HurtQuadrant.RIGHT;
			}
		}
		
		BroadcastMessage("GotHurtQuadrant", (int)hurtQuad);
		
		dataController.current -= damageSource.damageAmount;
		DeathCheck();
	}
	
	protected void DeathCheck() {
		if(dataController.current <= 0 && IsAlive) {
			IsAlive = false;
			
			// Show death text over screen
			if(deathText != null) {
				deathText.gameObject.SetActive(true);
			}
			
			MessengerControl.Broadcast("FadeIn", deathMusic);
			BroadcastMessage("PlayerDied");
			
			// Tell the AI that the player is dead
			Decoration[] decorations = GetComponentsInChildren<Decoration>();
			foreach(Decoration decoration in decorations) {
				if(decoration.aspect.aspectName.Equals(Constants.ASPECT_PLAYER)) {
					decoration.aspect.aspectName = Constants.ASPECT_DEAD_PLAYER;
				}
			}
		}
	}
}
