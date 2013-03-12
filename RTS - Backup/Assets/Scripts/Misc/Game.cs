using UnityEngine;
using System.Collections.Generic;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// The player controlled by this instance of the game (this client's player)
	public static Player MyPlayer { get; set; }
	[SerializeField]
	private Player thisPlayer; //this is a workaround so we can basically show a static var in the Unity inspector
	
	// Set of all players
	protected static HashSet<Player> players = new HashSet<Player>();
	
	// Whether or not the current game is running over a network
	public static bool IsMultiplayer { get; set; }
	[SerializeField]
	private bool multiplayerEnabled; //this is a workaround so we can basically show a static var in the Unity inspector
	
	// If the game is paused, no new commands should be able to be issued by the player
	public static bool Paused { get; set; } //TODO ! implement pausing in update loop of most scripts
	
	// The next id to be assigned to a new OwnedObject
	protected static int nextOwnedObjectId = 1;
	
	// Keeps track of all the OwnedObjects which have been created in the game, keyed by objectId
	protected static Dictionary<int, GameObject> ownedObjects = new Dictionary<int, GameObject>();
	
	void Awake() {
		MyPlayer = thisPlayer;
		IsMultiplayer = multiplayerEnabled;
	}
	
	void Start() {
		if(IsMultiplayer) {
			Paused = true;
			NetworkHub.Start();
			CommandHandler.Start();
		} else {
			Paused = false;
		}
	}
	
	void Update() {
		if(IsMultiplayer) {
			NetworkHub.Update();
			CommandHandler.Update();
		}
	}
	
	void OnDestroy() {
		if(IsMultiplayer) {
			NetworkHub.OnDestroy();
		}
	}
	
	// Adds a player to the hub, and also adds the player to their team
	public static void AddPlayer(Player player) {
		players.Add(player);
		player.team.players.Add(player);
	}
	
	public static void RegisterOwnedObject(OwnedObjectControl ownedObject) {
		ownedObject.OwnedObjectId = nextOwnedObjectId;
		ownedObjects.Add(ownedObject.OwnedObjectId, ownedObject.gameObject);
		nextOwnedObjectId++;
	}
	
	public static GameObject GetOwnedObjectById(int id) {
		return ownedObjects[id];
	}
}