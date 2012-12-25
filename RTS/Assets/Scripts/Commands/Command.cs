using System;
using Lidgren.Network;

// This class is the base for commands that a player may issue to affect the state of the game world
public abstract class Command {
	
	// The type of command this is so that it can be (de)serialized easily
	public CommandType commandType;
	// Id of the player who issued this command
	public int fromPlayer;
	// The turn on which this command should be executed
	public int turnToExecute;
	// Timestamp from when this command was issued
	public long timestamp;
	
	public Command(CommandType commandType) {
		this.commandType = commandType;
	}
}
