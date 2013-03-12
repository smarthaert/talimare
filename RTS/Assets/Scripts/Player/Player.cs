using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStatus))]
// Represents a single decision-making entity in the game. Can be human or computer, and can represent an in-game faction or nature
public class Player : MonoBehaviour {
	
	// This field exists essentially to cache the player status component (rather than Get it for every access)
	public PlayerStatus PlayerStatus { get; set; }
	
	// A map of relationships to every other player in the game
	public Dictionary<Player, PlayerRelationship> relationships = new Dictionary<Player, PlayerRelationship>();
	
	void Awake() {
		PlayerStatus = GetComponent<PlayerStatus>();
	}
	
	void Start() {
		Game.AddPlayer(this);
	}
}