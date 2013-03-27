using UnityEngine;
using System.Collections.Generic;

// This class is the base for all unit AI. It represents the ability for a unit to think and act on its own without input from any player (human player or computer player AI)
public class BasePersonalAI : MonoBehaviour {
	
	public AIState State { get; set; }
	
	public AIStance Stance { get; set; }
	public AIStance defaultStance = AIStance.Defensive;
	
	// Tracks how long the AI has been in its current state
	protected float StateTimer { get; set; }
	
	protected void Start() {
		
	}
	
	protected void Update() {
		UpdateState();
	}
	
	protected void UpdateState() {
		
	}
}
