using UnityEngine;
using System.Collections.Generic;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// If the game is paused, no new commands should be able to be issued by the player
	public static bool paused = false; //TODO implement pausing in update loop of most scripts
	
	// The next id to be assigned to a new object
	protected static int nextObjectId = 1;
	
	// Keeps track of all the objects which have been created in the game, keyed by objectId
	protected static Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();
	
	void Update() {
		CommandHandler.Update();
	}
	
	public static void RegisterSelectable(SelectableControl selectable) {
		selectable.objectId = nextObjectId;
		objects.Add(selectable.objectId, selectable.gameObject);
		nextObjectId++;
	}
	
	public static GameObject GetObjectById(int id) {
		return objects[id];
	}
}