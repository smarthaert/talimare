using System;

// This class is the base for synchronization or flow control messages between clients
public abstract class Message {

	// Id of the player who issued this message
	public int fromPlayer;
	// Timestamp from when this message was issued
	public long timestamp;
}
