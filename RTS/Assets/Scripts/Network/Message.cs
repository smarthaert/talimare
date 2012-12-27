using System;
using Lidgren.Network;

// This class is the base for synchronization or flow control messages between clients
public abstract class Message {
	
	// The type of message this is so that it can be (de)serialized easily
	public MessageType messageType;
	// Id of the player who issued this message
	public int fromPlayer;
	// Timestamp from when this message was issued
	public double timestamp;
	
	public Message(MessageType messageType) {
		this.messageType = messageType;
	}
	
	public virtual void SerializeTo(NetOutgoingMessage msg) {
		msg.writeWriteVariableInt32(fromPlayer);
		msg.WriteTime();
	}
	
	public virtual void DeserializeFrom(NetIncomingMessage msg) {
		fromPlayer = msg.ReadInt32();
		timestamp = msg.ReadTime();
	}
}
