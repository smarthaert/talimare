using UnityEngine;
using System;

// This class handles the queueing and execution of commands, as well as the turn synchronization system
public abstract class CommandHandler {
	
	// Whether or not the current game is running over a network
	protected static bool multiplayer = true;
	
	// The time each synchronization turn lasts, in ms
	public static float timePerTurn = 200f;
	// Number of turns to delay an incoming command
	protected static int commandTurnDelay = 2;
	
	// The current turn number
	protected static int currentTurn = 0;
	protected static float currentTurnTimer = 0f;
	
	public static void Update() {
		currentTurnTimer += Time.deltaTime;
		if(currentTurnTimer >= timePerTurn) {
			AdvanceTurn();
		}
	}
	
	protected static void AdvanceTurn() {
		currentTurn++;
		currentTurnTimer = 0f;
		//TODO dump the command queue for the new turn, executing them all? or is this done at the end of the turn?
	}
	
	public static void AddCommand(Command command) {
		if(multiplayer) {
			TagCommand(command);
			//TODO add command to local queue and also send it across the network
		} else {
			ExecuteCommand(command);
		}
	}
	
	protected static void TagCommand(Command command) {
		command.turnToExecute = currentTurn + commandTurnDelay;
		command.timestamp = DateTime.UtcNow;
	}
	
	protected static void ExecuteCommand(Command command) {
		
	}
}