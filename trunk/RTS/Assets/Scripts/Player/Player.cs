using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStatus))]
public class Player : MonoBehaviour {
	
	public Team team;
	
	[NonSerialized]
	public PlayerStatus playerStatus;
	[NonSerialized]
	public PlayerInput playerInput;
	
	//TODO !!! add a player relationship dictionary before continuing work on players
	
	void Awake() {
		playerStatus = GetComponent<PlayerStatus>();
		playerInput = GetComponent<PlayerInput>();
	}
	
	void Start() {
		if(team == null)
			Debug.Log("Player: "+name+" has no team! Team must be set immediately after instantiating the player.");
		PlayerHub.AddPlayer(this);
	}
}