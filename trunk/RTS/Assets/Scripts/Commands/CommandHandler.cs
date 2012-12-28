using UnityEngine;
using System;
using System.Collections.Generic;
using Lidgren.Network;

// This class handles the queueing and execution of commands, as well as the turn synchronization system
public abstract class CommandHandler {
	
	// Time each synchronization turn lasts, in seconds
	public static float timePerTurn = 0.2f;
	
	// Queue for commands for future turns (outer keyed by turn #, middle keyed by player id, inner keyed by sequence)
	protected static Dictionary<int, SortedList<int, SortedList<int, Command>>> commandQueue = new Dictionary<int, SortedList<int, SortedList<int, Command>>>();
	// Holdes 'turn done' messages from other players (outer keyed by turn #)
	protected static Dictionary<int, List<TurnDoneMessage>> turnDoneMessages = new Dictionary<int, List<TurnDoneMessage>>();
	
	// Current turn tracking variables
	protected static int currentTurn = -1;
	protected static float currentTurnTimer = 0f;
	protected static int currentTurnCommandSequence = 0;
	
	public static void Start() {
		// If this is the first player in the game, wait for others
		if(NetworkHub.GetNumPeers() == 0) {
			Debug.Log("Game paused, waiting for other players...");
			Game.paused = true;
		}
	}
	
	// Only called during a multiplayer game
	public static void Update() {
		if(!Game.paused) {
			currentTurnTimer += Time.deltaTime;
			if(currentTurnTimer >= timePerTurn) {
				FinishTurn();
				TryAdvanceTurn();
			}
		}
	}
	
	// Finishes the current turn and transmits a 'turn done' message
	protected static void FinishTurn() {
		TurnDoneMessage turnDoneMessage = new TurnDoneMessage(currentTurn + 2, currentTurnCommandSequence);
		TagMessage(turnDoneMessage);
		NetworkHub.SendMessage(turnDoneMessage);
	}
	
	// Attempts to advance to the next synchronization turn if all commands for that turn have been received from all players
	protected static void TryAdvanceTurn() {
		if(HaveAllCommandsForTurn(currentTurn+1)) {
			currentTurn++;
			
			if(commandQueue.ContainsKey(currentTurn)) {
				//TODO ensure that the command queue is being ordered properly
				// Dump the command queue for the new turn, executing them all
				IList<SortedList<int, Command>> commandsByPlayer = commandQueue[currentTurn].Values;
				foreach(SortedList<int, Command> sequencedCommands in commandsByPlayer) {
					foreach(Command command in sequencedCommands.Values) {
						CommandExecutor.ExecuteCommand(command);
					}
				}
				commandQueue.Remove(currentTurn);
				turnDoneMessages.Remove(currentTurn);
			}
			
			// Advance to the next turn, ready to accept new commands
			currentTurnTimer = 0f;
			currentTurnCommandSequence = 0;
			Game.paused = false;
		} else {
			// If waiting for another player's commands, pause our game and prevent issuing more commands
			Game.paused = true;
			Debug.Log("waiting for commands, pausing game...");
			//TODO Process drop and timeout checks, find out which player is holding us up
		}
	}
	
	// Returns whether or not all commands have been received from all other players for the given turn
	protected static bool HaveAllCommandsForTurn(int turn) {
		if(turn < 1)
			return true; // We're in an initiation turn; automatically continue
		if(!turnDoneMessages.ContainsKey(turn))
			return false; // No 'turn done' messages have been received yet for this turn
		if(turnDoneMessages[turn].Count != NetworkHub.GetNumPeers())
			return false; // Don't have all 'turn done' messages for this turn
		
		// We have all 'turn done' messages, so let's make sure we have all the commands from each player
		if(commandQueue.ContainsKey(turn)) {
			// We have some commands for this turn
			foreach(TurnDoneMessage turnDoneMessage in turnDoneMessages[turn]) {
				if(commandQueue[turn].ContainsKey(turnDoneMessage.fromPlayer)) {
					// We have some commands for this player
					if(commandQueue[turn][turnDoneMessage.fromPlayer].Count < turnDoneMessage.numCommands) {
						Debug.Log("Have all turn dones, but missing commands from player: "+turnDoneMessage.fromPlayer);
						return false;
					} else if(commandQueue[turn][turnDoneMessage.fromPlayer].Count > turnDoneMessage.numCommands) {
						Debug.Log("Have all turn dones, but somehow have more commands than player: "+turnDoneMessage.fromPlayer+" sent. This should never happen.");
						return false;
					}
				}
			}
		}
		return true;
	}
	
	// Add a command, received from this player, to be executed immediately in singleplayer or queued in multiplayer
	public static void AddCommandFromLocal(Command command) {
		if(Game.multiplayer) {
			if(Game.paused) {
				Debug.Log("A local command was received between turns. This should never happen. Is some script unpaused?");
			} else {
				TagCommand(command);
				QueueCommand(command);
				NetworkHub.SendMessage(command);
			}
		} else {
			CommandExecutor.ExecuteCommand(command);
		}
	}
	
	// Add a command, received from the network, to be executed in the future
	public static void AddCommandFromNetwork(Command command) {
		QueueCommand(command);
		
		// If the game is paused and we're waiting on something, see if this is what we needed
		if(Game.paused) {
			TryAdvanceTurn();
		}
	}
	
	// Tags a command with all required header values
	protected static void TagCommand(Command command) {
		TagMessage(command);
		command.turnToExecute = currentTurn + 2;
		command.sequence = ++currentTurnCommandSequence;
	}
	
	// Tags a message with all required header values
	protected static void TagMessage(Message message) {
		message.fromPlayer = PlayerHub.myPlayer.id;
	}
	
	// Queues a command to be executed some time in the future
	protected static void QueueCommand(Command command) {
		if(!commandQueue.ContainsKey(command.turnToExecute)) {
			commandQueue.Add(command.turnToExecute, new SortedList<int, SortedList<int, Command>>());
		}
		if(!commandQueue[command.turnToExecute].ContainsKey(command.fromPlayer)) {
			commandQueue[command.turnToExecute].Add(command.fromPlayer, new SortedList<int, Command>());
		}
		Debug.Log("turn: "+currentTurn+" turn to exe: "+command.turnToExecute+" sequence: "+command.sequence);
		commandQueue[command.turnToExecute][command.fromPlayer].Add(command.sequence, command);
	}
	
	// Stores a 'turn done' message received from the network
	public static void StoreTurnDoneMessage(TurnDoneMessage turnDoneMessage) {
		if(!turnDoneMessages.ContainsKey(turnDoneMessage.turn)) {
			turnDoneMessages.Add(turnDoneMessage.turn, new List<TurnDoneMessage>());
		}
		turnDoneMessages[turnDoneMessage.turn].Add(turnDoneMessage);
		
		// If the game is paused and we're waiting on something, see if this is what we needed
		if(Game.paused) {
			TryAdvanceTurn();
		}
	}
}