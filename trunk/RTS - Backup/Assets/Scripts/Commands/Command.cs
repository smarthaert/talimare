using System;
using Lidgren.Network;

// This class is the base for commands that a player may issue to affect the state of the game world
public abstract class Command : Message {
	
	// The turn on which this command should be executed
	public int turnToExecute;
	// The sequence number of this command in its turn for the issuing player
	public int sequence;
	
	public Command(MessageType messageType) : base(messageType) {}
	
	public override void SerializeTo(NetOutgoingMessage msg) {
		base.SerializeTo(msg);
		msg.Write(turnToExecute);
		msg.Write(sequence);
	}
	
	public override void DeserializeFrom(NetIncomingMessage msg) {
		base.DeserializeFrom(msg);
		turnToExecute = msg.ReadInt32();
		sequence = msg.ReadInt32();
	}
}
