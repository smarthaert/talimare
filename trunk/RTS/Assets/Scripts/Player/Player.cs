using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStatus))]
public class Player : MonoBehaviour {
	
	[NonSerialized]
	public PlayerStatus playerStatus;
	[NonSerialized]
	public PlayerInput playerInput;
	
	//TODO !!! add a player relationship dictionary before continuing work on players
	
	void Awake() {
		playerStatus = GetComponent<PlayerStatus>();
		playerInput = GetComponent<PlayerInput>();
	}
}