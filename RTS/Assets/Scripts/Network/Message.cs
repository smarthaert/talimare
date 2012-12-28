using System;
using Lidgren.Network;

// This class is the base for synchronization or flow control messages between clients
public abstract class Message {
	
	// The type of message this is so that it can be (de)serialized easily.
	// Doesn't need to be serialized itself because it is populated at construction time
	public MessageType messageType;
	// Id of the player who issued this message
	public int fromPlayer;
	
	public Message(MessageType messageType) {
		this.messageType = messageType;
	}
	
	public virtual void SerializeTo(NetOutgoingMessage msg) {
		msg.Write((int)messageType);
		msg.Write(fromPlayer);
	}
	
	public virtual void DeserializeFrom(NetIncomingMessage msg) {
		fromPlayer = msg.ReadInt32();
	}
}
