using System;
using Lidgren.Network;

// This class is the base for commands that a player may issue to affect the state of the game world
public abstract class Command : Message {
	
	// The turn on which this command should be executed
	public int turnToExecute;
	
	public Command(MessageType messageType) : base(messageType) {}
	
	public override void SerializeTo(NetOutgoingMessage msg) {
		base.SerializeTo(msg);
		msg.Write(turnToExecute);
	}
	
	public override void DeserializeFrom(NetIncomingMessage msg) {
		base.DeserializeFrom(msg);
		turnToExecute = msg.ReadInt32();
	}
}
