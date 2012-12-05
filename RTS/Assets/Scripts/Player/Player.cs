using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStatus))]
public class Player : MonoBehaviour {
	
	[NonSerialized]
	public PlayerStatus playerStatus;
	[NonSerialized]
	public PlayerInput playerInput;
	
	void Awake() {
		playerStatus = GetComponent<PlayerStatus>();
		playerInput = GetComponent<PlayerInput>();
	}
}