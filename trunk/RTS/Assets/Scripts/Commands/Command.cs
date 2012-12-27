using System;
using Lidgren.Network;

// This class is the base for commands that a player may issue to affect the state of the game world
public abstract class Command : Message {
	
	// The turn on which this command should be executed
	public int turnToExecute;
	
	public Command(MessageType messageType) : base(messageType) {}
	
	public virtual void SerializeTo(NetOutgoingMessage msg) {
		msg.writeWriteVariableInt32(fromPlayer);
		msg.WriteTime();
	}
	
	public virtual void DeserializeFrom(NetIncomingMessage msg) {
		fromPlayer = msg.ReadInt32();
		timestamp = msg.ReadTime();
	}
}
