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
	
	// Creates a new instance of the given Controllable for the given Player at the given position.
	// Applies applicable techs and other necessary functions
	public static GameObject Instantiate(Controllable controllable, Player player, Vector3 position) {
		GameObject newObject = (GameObject)Instantiate(controllable.gameObject, position, Quaternion.identity);
		Controllable newControllable = newObject.GetComponent<Controllable>();
		newControllable.owner = player;
		newControllable.name = controllable.gameObject.name;
		//apply all applicable techs to the new object
		PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
		foreach(Tech appliedTech in newControllable.applicableTechs) {
			if(playerStatus.techs.Contains(appliedTech)) {
				appliedTech.ApplyTechTo(newObject);
			}
		}
		return newObject;
	}
}