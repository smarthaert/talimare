using System;

// This class is used to signal that a player has finished sending all of his commands for a turn
public class TurnDoneMessage : Message {

	// The turn which the player completed
	public int turn;
	// The total number of commands that were issued for the turn
	public int numCommands;
	
	public TurnDoneMessage(int turn, int numCommands) : base(MessageType.TurnDoneMessage) {
		this.turn = turn;
		this.numCommands = numCommands;
	}
}
