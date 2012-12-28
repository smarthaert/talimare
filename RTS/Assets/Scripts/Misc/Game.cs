using UnityEngine;
using System.Collections.Generic;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// Whether or not the current game is running over a network
	public static bool multiplayer = true;
	
	// If the game is paused, no new commands should be able to be issued by the player
	public static bool paused = false; //TODO ! implement pausing in update loop of most scripts
	
	// The next id to be assigned to a new OwnedObject
	protected static int nextOwnedObjectId = 1;
	
	// Keeps track of all the OwnedObjects which have been created in the game, keyed by objectId
	protected static Dictionary<int, GameObject> ownedObjects = new Dictionary<int, GameObject>();
	
	void Start() {
		if(multiplayer) {
			NetworkHub.Start();
			CommandHandler.Start();
		}
	}
	
	void Update() {
		if(multiplayer) {
			NetworkHub.Update();
			CommandHandler.Update();
		}
	}
	
	void OnDestroy() {
		if(multiplayer) {
			NetworkHub.OnDestroy();
		}
	}
	
	public static void RegisterOwnedObject(OwnedObjectControl ownedObject) {
		ownedObject.ownedObjectId = nextOwnedObjectId;
		ownedObjects.Add(ownedObject.ownedObjectId, ownedObject.gameObject);
		nextOwnedObjectId++;
		
		//TODO ! dynamically add AIVision components with correct settings based on the owner of this object
		//and PlayerHub.myPlayer.id
	}
	
	public static GameObject GetOwnedObjectById(int id) {
		return ownedObjects[id];
	}
}