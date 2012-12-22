using System;

// This class is the base for commands that a player may issue to affect the state of the game world
public abstract class Command {
	
	// Id of the player who issued this command
	public int fromPlayer;
	// The turn on which this command should be executed
	public int turnToExecute;
	// The sequence number of this command within its turnToExecute
	public int sequence;
	// Timestamp from when this command was issued
	public long timestamp;
}
