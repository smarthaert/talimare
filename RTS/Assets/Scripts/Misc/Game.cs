using UnityEngine;
using System.Collections.Generic;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// The human player controlled by this instance of the game
	public static Player ThisPlayer { get; set; }
	
	// Set of all players
	protected static HashSet<Player> players = new HashSet<Player>();
	
	void Awake() {
		ThisPlayer = (Player)GameObject.FindSceneObjectsOfType(typeof(HumanPlayer))[0];
	}
	
	// Adds a player to the game
	public static void AddPlayer(Player newPlayer) {
		foreach(Player existingPlayer in players) {
			existingPlayer.relationships.Add(newPlayer, PlayerRelationship.HOSTILE);
			newPlayer.relationships.Add(existingPlayer, PlayerRelationship.HOSTILE);
		}
		players.Add(newPlayer);
	}
}