using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStatus))]
public class Player : MonoBehaviour {
	
	public int id;
	public Team team;
	
	[NonSerialized]
	public PlayerStatus playerStatus;
	[NonSerialized]
	public PlayerInput playerInput;
	
	void Start() {
		playerStatus = GetComponent<PlayerStatus>();
		playerInput = GetComponent<PlayerInput>();
			
		if(team == null)
			Debug.Log("Player: "+name+" has no team! Team must be set immediately after instantiating the player.");
		Game.AddPlayer(this);
	}
}