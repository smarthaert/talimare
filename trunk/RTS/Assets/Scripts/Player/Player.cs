using UnityEngine;
using System.Collections.Generic;

// Represents a single decision-making entity in the game. Can be human or computer, and can represent an in-game faction or nature
[RequireComponent(typeof(PlayerStatus))]
[RequireComponent(typeof(StrategicAI))]
public class Player : MonoBehaviour {
	
	public PlayerStatus PlayerStatus { get; protected set; }
	public StrategicAI StrategicAI { get; protected set; }
	
	// A map of relationships to every other player in the game
	public Dictionary<Player, PlayerRelationship> Relationships;
	
	protected virtual void Awake() {
		Relationships = new Dictionary<Player, PlayerRelationship>();
		//TODO SET all simple cached references in awake. instantiate all simple objects in awake. start is for more complex functions
		PlayerStatus = GetComponent<PlayerStatus>();
		StrategicAI = GetComponent<StrategicAI>();
	}
	
	protected virtual void Start() {
		Game.AddPlayer(this);
	}
}