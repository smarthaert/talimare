using UnityEngine;
using System;
using System.Collections.Generic;

// This class handles the queueing and execution of commands, as well as the turn synchronization system
public abstract class CommandHandler {
	
	// Whether or not the current game is running over a network
	protected static bool multiplayer = true;
	
	// Time each synchronization turn lasts, in ms
	public static float timePerTurn = 200f;
	
	// Queue for commands for future turns (outer keyed by turn #, inner keyed by timestamp)
	protected static Dictionary<int, SortedList<long, Command>> commandQueue = new Dictionary<int, SortedList<long, Command>>();
	// Holds the number of commands received from other players (outer keyed by turn #, inner keyed by player id)
	protected static Dictionary<int, Dictionary<int, int>> numCommands = new Dictionary<int, Dictionary<int, int>>();
	// Holdes 'turn done' messages from other players (outer keyed by turn #)
	protected static Dictionary<int, List<TurnDoneMessage>> turnDoneMessages = new Dictionary<int, List<TurnDoneMessage>>();
	
	// Current turn tracking variables
	protected static int currentTurn = 1;
	protected static float currentTurnTimer = 0f;
	protected static int currentTurnCommandSequence = 0;
	protected static bool currentTurnFinished = false;
	
	public static void Update() {
		if(multiplayer) {
			if(currentTurnFinished) {
				TryAdvanceTurn();
			} else {
				currentTurnTimer += Time.deltaTime;
				if(currentTurnTimer >= timePerTurn) {
					FinishTurn();
					TryAdvanceTurn();
				}
			}
		}
	}
	
	// Finishes the current turn and transmits a 'done' message
	protected static void FinishTurn() {
		TurnDoneMessage turnDoneMessage = new TurnDoneMessage(currentTurn, currentTurnCommandSequence);
		TagMessage(turnDoneMessage);
		NetworkHub.SendMessage(turnDoneMessage);
		
		currentTurn++;
		currentTurnTimer = 0f;
		currentTurnCommandSequence = 0;
		currentTurnFinished = true;
	}
	
	// Attempts to advance to the next synchronization turn if all commands for that turn have been received from all players
	protected static void TryAdvanceTurn() {
		// If received 'turn done' messages from all other players for new turn
		if(HaveAllDonesForCurrentTurn()) {
			//TODO ensure that the command queue is being ordered properly
			// Dump the command queue for the new turn, executing them all
			IList<Command> newTurnCommands = commandQueue[currentTurn].Values;
			while(newTurnCommands.Count > 0) {
				CommandExecutor.ExecuteCommand(newTurnCommands[0]);
				newTurnCommands.RemoveAt(0);
			}
			commandQueue.Remove(currentTurn);
			numCommands.Remove(currentTurn);
			turnDoneMessages.Remove(currentTurn);
			
			// Advance to the next turn, ready to accept new commands
			Game.paused = false;
			currentTurnFinished = false;
		} else {
			// If waiting for another player's commands, pause our game and prevent issuing more commands
			Game.paused = true;
			
			//TODO Process drop and timeout checks, find out which player is holding us up
		}
	}
	
	// Returns whether or not all 'turn done' messages have been received from all other players for the current turn
	protected static bool HaveAllDonesForCurrentTurn() {
		if(turnDoneMessages[currentTurn].Count == NetworkHub.GetNumOtherClients()) {
			// We have all the turn done messages, so let's make sure we have all the commands from each player
			foreach(TurnDoneMessage turnDoneMessage in turnDoneMessages[currentTurn]) {
				if(numCommands[currentTurn][turnDoneMessage.fromPlayer] < turnDoneMessage.numCommands) {
					Debug.Log("Have all turn dones, but missing commands from player: "+turnDoneMessage.fromPlayer);
					return false;
				} else if(numCommands[currentTurn][turnDoneMessage.fromPlayer] > turnDoneMessage.numCommands) {
					Debug.Log("Have all turn dones, but somehow have more commands than player: "+turnDoneMessage.fromPlayer+" sent.\nThis should never happen.");
					return false;
				}
			}
			return true;
		} else {
			return false;
		}
	}
	
	// Add a command, received from this player, to be executed immediately in singleplayer or queued in multiplayer
	public static void AddCommandFromLocal(Command command) {
		if(multiplayer) {
			if(currentTurnFinished) {
				Debug.Log("A local command was received between turns. This should never happen. Is some script unpaused?");
			} else {
				TagCommand(command);
				QueueCommand(command);
				NetworkHub.SendCommand(command);
			}
		} else {
			CommandExecutor.ExecuteCommand(command);
		}
	}
	
	// Add a command, received from the network, to be executed in the future
	public static void AddCommandFromNetwork(Command command) {
		QueueCommand(command);
		// Keep track of the number of commands received from each player for easy reference later
		if(!numCommands.ContainsKey(command.turnToExecute)) {
			numCommands.Add(command.turnToExecute, new Dictionary<int, int>());
		}
		if(!numCommands[command.turnToExecute].ContainsKey(command.fromPlayer)) {
			numCommands[command.turnToExecute].Add(command.fromPlayer, 1);
		} else {
			numCommands[command.turnToExecute][command.fromPlayer]++;
		}
	}
	
	// Tags a command with all required header values
	protected static void TagCommand(Command command) {
		command.fromPlayer = PlayerHub.myPlayer.id;
		command.turnToExecute = currentTurn + 2;
		command.sequence = ++currentTurnCommandSequence;
		command.timestamp = DateTime.UtcNow.Ticks;
	}
	
	// Tags a message with all required header values
	protected static void TagMessage(Message message) {
		message.fromPlayer = PlayerHub.myPlayer.id;
		message.timestamp = DateTime.UtcNow.Ticks;
	}
	
	// Queues a command to be executed some time in the future
	protected static void QueueCommand(Command command) {
		if(!commandQueue.ContainsKey(command.turnToExecute)) {
			commandQueue.Add(command.turnToExecute, new SortedList<long, Command>());
		}
		commandQueue[command.turnToExecute].Add(command.timestamp, command);
	}
	
	// Stores a 'turn done' message received from the network
	public static void StoreTurnDoneMessage(TurnDoneMessage turnDoneMessage) {
		if(!turnDoneMessages.ContainsKey(turnDoneMessage.turn)) {
			turnDoneMessages.Add(turnDoneMessage.turn, new List<TurnDoneMessage>());
		}
		turnDoneMessages[turnDoneMessage.turn].Add(turnDoneMessage);
	}
}