using UnityEngine;
using System;

// Defines the behavior of a Selectable which is owned by a player
public abstract class OwnedObjectControl : SelectableControl {
	
	// Generated id for an owned object
	[NonSerialized]
	public int ownedObjectId;
	
	// This is the object's major reference point to its Player object, aka the object's owner.
	// All other scripts on the object should retrieve the Player reference from here
	public Player player;
	
	protected override void Start() {
		if(player == null)
			Debug.Log("Player was never set for the OwnedObject: "+name+". It should be set immediately after instantiating the object.");
		
		Game.RegisterOwnedObject(this);
	}
}

