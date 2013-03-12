using UnityEngine;
using System;

// Defines the behavior of a Selectable which is owned by a player
public abstract class OwnedObjectControl : SelectableControl {
	
	// This is the object's major reference point to its Player object, aka the object's owner.
	// All other scripts on the object should retrieve the Player reference from here
	public Player player;
	
	protected override void Start() {
		base.Start();
		
		if(player == null)
			Debug.Log("Player was never set for the OwnedObject: "+name+". It should be set immediately after instantiating the object.");
		
		ConfigureVisionSettings();
	}
	
	// Configures this object's vision settings based on its owner
	protected void ConfigureVisionSettings() {
		AIVision visionComponent = gameObject.GetComponentInChildren<AIVision>();
		if(visionComponent != null) {
			if(player == Game.ThisPlayer)
				visionComponent.RevealsFog = true;
			else
				visionComponent.RevealsFog = false;
			
			if(gameObject.CompareTag("Unit"))
				visionComponent.HidesInFog = true;
			else
				visionComponent.HidesInFog = false;
		}
	}
}
