using UnityEngine;
using System.Collections.Generic;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// The human player controlled by this instance of the game
	public static Player ThisPlayer { get; protected set; }
	
	// A reference to the PlayerInput component in case any other scripts need it
	public static PlayerInput PlayerInput { get; protected set; }
	
	// Set of all players currently active in the game
	protected static HashSet<Player> players = new HashSet<Player>();
	
	protected void Awake() {
		PlayerInput = GetComponent<PlayerInput>();
	}
	
	protected void Start() {
		ThisPlayer = (Player)GameObject.FindSceneObjectsOfType(typeof(HumanPlayer))[0];
	}
	
	// Adds a player to the game, complete with player relationships
	public static void AddPlayer(Player newPlayer) {
		foreach(Player existingPlayer in players) {
			existingPlayer.Relationships.Add(newPlayer, PlayerRelationship.HOSTILE);
			newPlayer.Relationships.Add(existingPlayer, PlayerRelationship.HOSTILE);
		}
		players.Add(newPlayer);
	}
}