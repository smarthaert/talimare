using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStatus))]
// Represents a single decision-making entity in the game. Can be human or computer, and can represent an in-game faction or nature
public class Player : MonoBehaviour {
	
	public int id;
	public Team team;
	
	// This field exists essentially to cache the player status component (rather than find it for every access)
	public PlayerStatus playerStatus { get; set; }
	
	void Awake() {
		playerStatus = GetComponent<PlayerStatus>();
	}
	
	void Start() {
		if(team == null)
			Debug.Log("Player: "+name+" has no team! Team must be set immediately after instantiating the player.");
		Game.AddPlayer(this);
	}
}